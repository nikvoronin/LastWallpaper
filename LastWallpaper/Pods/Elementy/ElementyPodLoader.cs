﻿using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Models.Rss;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing;

public sealed class ElementyPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    IFeedReader<RssFeed> feedReader )
    : HttpPodLoader( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Elementy ).ToLower();

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        var feedResult =
            await _feedReader.ParseFeedAsync( RssFeedUrl, ct );

        if (feedResult.IsFailed)
            return Result.Fail( "Failed to update an RSS feed." );

        var lastItem = feedResult.Value.Channel.Items[0];

        if (_resourceManager.PotdExists( Name, lastItem.PubDate ))
            return Result.Fail( "Picture is already known." );

        if (lastItem.Enclosure.Type != "image/jpeg")
            return Result.Fail(
                $"The media type '{lastItem.Enclosure.Type}' is not supported." );

        var hdUrl = ConvertToHdFileUrl( lastItem.Enclosure.Url );

        var cachedImageFilename =
            (await DownloadToTemporaryFileAsync( hdUrl, ct ))
            .ValueOrDefault;

        if (cachedImageFilename is null)
            return Result.Fail(
                $"Can not download media from {hdUrl}." );

        var result = new Imago() {
            PodName = Name,
            Filename = cachedImageFilename,
            Created = lastItem.PubDate.Date + DateTime.Now.TimeOfDay,
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