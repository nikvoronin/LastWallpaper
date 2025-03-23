using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing.Fetchers;
using LastWallpaper.Pods.Bing.Models;
using System.Net.Http;

namespace LastWallpaper.Pods.Bing;

public sealed class BingPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    BingSettings settings )
    : PodLoader<BingPodNews>(
        PodType.Bing,
        new BingNewsFetcher(
            httpClient,
            settings ),
        new HttpPotdFetcher<BingPodNews>(
            httpClient,
            new BingDescriptionFetcher(),
            resourceManager ),
        resourceManager )
{ }
