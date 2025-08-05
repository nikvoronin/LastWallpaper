using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Wikimedia.Fetchers;
using LastWallpaper.Pods.Wikimedia.Models;
using System.Net.Http;

namespace LastWallpaper.Pods.Wikimedia;

public sealed class WikipediaPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<WikipediaPodNews>(
        PodType.Wikipedia,
        new WikipediaNewsFetcher( httpClient ),
        new HttpPotdFetcher<WikipediaPodNews>(
            httpClient,
            new WikipediaDescriptionFetcher( httpClient ),
            resourceManager ),
        resourceManager )
{ }
