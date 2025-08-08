using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Copernicus.Fetchers;
using System.Net.Http;

namespace LastWallpaper.Pods.Copernicus;

public sealed class CopernicusPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<HtmlPodNews>(
        PodType.Copernicus,
        new HtmlNewsFetcher<HtmlPodNews>(
            httpClient,
            new( CopernicusImageDayUrl ),
            new CopernicusNewsExtractor(
                new( CopernicusSystemFilesUrl ) ) ),
        new HttpPotdFetcher<HtmlPodNews>(
            httpClient,
            new HtmlDescriptionFetcher(),
            resourceManager ),
        resourceManager )
{
    private const string CopernicusBaseUrl = "https://www.copernicus.eu";
    private const string CopernicusImageDayUrl = CopernicusBaseUrl + "/en/media/image-day";
    private const string CopernicusSystemFilesUrl = CopernicusBaseUrl + "/system/files";
}
