using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Astrobin.Fetchers;
using System.Net.Http;

namespace LastWallpaper.Pods.Astrobin;

public sealed class AstrobinPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<HtmlPodNews>(
        PodType.Astrobin,
        new HtmlNewsFetcher<HtmlPodNews>(
            httpClient,
            new( AstrobinIotdArchiveUrl ),
            new AstrobinNewsExtractor(
                new( AstrobinBaseUrl ) ) ),
        new HttpPotdFetcher<HtmlPodNews>(
            httpClient,
            new AstrobinDescriptionFetcher( httpClient ),
            resourceManager ),
        resourceManager )
{
    private const string AstrobinIotdArchiveUrl = AstrobinBaseUrl + "/iotd/archive/";
    private const string AstrobinBaseUrl = "https://www.astrobin.com";
}
