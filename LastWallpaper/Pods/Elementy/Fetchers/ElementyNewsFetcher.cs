using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Models.Rss;
using LastWallpaper.Pods.Elementy.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Elementy.Fetchers;

public sealed class ElementyNewsFetcher : INewsFetcher<ElementyPodNews>
{
    public ElementyNewsFetcher(
        HttpClient httpClient,
        IFeedReader<RssFeed> feedReader,
        Uri elementyRssFeedUri )
    {
        ArgumentNullException.ThrowIfNull( httpClient );
        ArgumentNullException.ThrowIfNull( feedReader );
        ArgumentNullException.ThrowIfNull( elementyRssFeedUri );

        _httpClient = httpClient;
        _rssFeedUri = elementyRssFeedUri;
        _feedReader = feedReader;
    }

    public async Task<Result<ElementyPodNews>> FetchNewsAsync( CancellationToken ct )
    {
        var feedResult =
            await _feedReader.ParseFeedAsync( _rssFeedUri, _httpClient, ct );

        if (feedResult.IsFailed)
            return Result.Fail( "Failed to update an RSS feed." );

        var lastItem = feedResult.Value.Channel.Items[0];

        if (lastItem.Enclosure.Type != "image/jpeg")
            return Result.Fail(
                $"The media type '{lastItem.Enclosure.Type}' is not supported." );

        return Result.Ok(
            new ElementyPodNews() {
                PodType = PodType.Elementy,
                PubDate = lastItem.PubDate.Date,
                Item = lastItem
            } );
    }

    private readonly HttpClient _httpClient;
    private readonly Uri _rssFeedUri;
    private readonly IFeedReader<RssFeed> _feedReader;
}
