using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Models.Rss;
using LastWallpaper.Pods.Elementy.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Elementy;

public sealed class ElementyPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    IFeedReader<RssFeed> feedReader )
    : HttpPodLoader<ElementyPodLatestUpdate>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Elementy ).ToLower();

    protected async override Task<Result<ElementyPodLatestUpdate>> FetchLatestUpdateInternalAsync(
        CancellationToken ct )
    {
        var feedResult =
            await _feedReader.ParseFeedAsync( RssFeedUrl, _httpClient, ct );

        if (feedResult.IsFailed)
            return Result.Fail( "Failed to update an RSS feed." );

        var lastItem = feedResult.Value.Channel.Items[0];

        if (lastItem.Enclosure.Type != "image/jpeg")
            return Result.Fail(
                $"The media type '{lastItem.Enclosure.Type}' is not supported." );

        return Result.Ok(
            new ElementyPodLatestUpdate() {
                PubDate = lastItem.PubDate.Date,
                Item = lastItem
            } );
    }

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        ElementyPodLatestUpdate latestUpdate,
        CancellationToken ct )
    {
        var lastItem = latestUpdate.Item;

        var hdUrl = ConvertToHdFileUrl( lastItem.Enclosure.Url );

        var cachedFilenameResult =
            await DownloadFileAsync( hdUrl, ct );

        if (cachedFilenameResult.IsFailed)
            return Result.Fail(
                $"Can not download media from {hdUrl}." );

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedFilenameResult.Value,
            Created = lastItem.PubDate.Date,
            Title = $"{lastItem.Title}. {lastItem.Category}.",
            Copyright = lastItem.Description,
        };

        return Result.Ok( result );
    }

    public static string ConvertToHdFileUrl( string url )
    {
        var uri = new Uri( url );
        var filename = uri.Segments[^1];
        var hdFilename =
            filename[..filename.LastIndexOf( '_' )]
            + new FileInfo( filename ).Extension;

        return $"{uri.Scheme}://{uri.Host}{string.Concat( uri.Segments[..^1] )}{hdFilename}";
    }

    private readonly IFeedReader<RssFeed> _feedReader = feedReader;

    private const string RssFeedUrl = "https://elementy.ru/rss/kartinka_dnya";
}
