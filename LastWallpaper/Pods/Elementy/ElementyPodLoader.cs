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
    : HttpPodLoader<ElementyPodNews>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Elementy ).ToLower();

    protected async override Task<Result<ElementyPodNews>> FetchNewsInternalAsync(
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
            new ElementyPodNews() {
                PubDate = lastItem.PubDate.Date,
                Item = lastItem
            } );
    }

    public static Uri ToHdImageUrl( string url )
    {
        var uri = new Uri( url );
        var filename = uri.Segments[^1];
        var hdFilename =
            filename[..filename.LastIndexOf( '_' )]
            + new FileInfo( filename ).Extension;

        return
            new Uri(
                $"{uri.Scheme}://{uri.Host}{string.Concat( uri.Segments[..^1] )}{hdFilename}" );
    }

    protected override Task<Result<PotdDescription>> GetDescriptionAsync(
        ElementyPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                Url = ToHdImageUrl( news.Item.Enclosure.Url ),
                PubDate = news.Item.PubDate.Date,
                Title = $"{news.Item.Title}. {news.Item.Category}.",
                Copyright = news.Item.Description,
            } ) );

    private readonly IFeedReader<RssFeed> _feedReader = feedReader;

    private const string RssFeedUrl = "https://elementy.ru/rss/kartinka_dnya";
}