using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Models.Rss;
using LastWallpaper.Pods.Bing;
using LastWallpaper.Pods.Elementy;
using LastWallpaper.Pods.Nasa;
using LastWallpaper.Pods.Wikimedia;
using System.Net.Http;

namespace LastWallpaper.Pods;

public static class PodsFactory
{
    public static IPotdLoader? Create(
        PodType podType,
        HttpClient client,
        IResourceManager resourceManager,
        IFeedReader<RssFeed> rssReader,
        AppSettings settings )
        => podType switch {

            PodType.Bing =>
                new BingPodLoader(
                    client,
                    resourceManager,
                    settings.BingOptions ),

            PodType.Apod =>
                new NasaApodLoader(
                    client,
                    resourceManager,
                    settings.ApodOptions ),

            PodType.Wikipedia =>
                new WikipediaPodLoader(
                    client,
                    resourceManager ),

            PodType.Elementy =>
                new ElementyPodLoader(
                    client,
                    resourceManager,
                    rssReader ),

            _ => null
        };
}
