using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Models.Rss;
using LastWallpaper.Pods.Elementy.Fetchers;
using LastWallpaper.Pods.Elementy.Models;
using System.Net.Http;

namespace LastWallpaper.Pods.Elementy;

public sealed class ElementyPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    IFeedReader<RssFeed> feedReader )
    : PodLoader<ElementyPodNews>(
        PodType.Elementy,
        new ElementyNewsFetcher(
            httpClient,
            feedReader,
            new( "https://elementy.ru/rss/kartinka_dnya" ) ),
        new HttpPotdFetcher<ElementyPodNews>(
            httpClient,
            new ElementyDescriptionFetcher(),
            resourceManager ),
        resourceManager )
{ }