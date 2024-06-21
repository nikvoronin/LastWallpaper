using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing;
using LastWallpaper.Pods.Nasa;
using LastWallpaper.Pods.Wikimedia;
using System.Net.Http;

namespace LastWallpaper.Pods;

public static class PodFabric
{
    public static IPotdLoader? CreatePod(
        PodType podType,
        HttpClient client,
        AppSettings settings )
        => podType switch {
            PodType.Bing =>
                new BingPodLoader( client, settings.BingOptions ),
            PodType.Apod =>
                new NasaApodLoader( client, settings.ApodOptions ),
            PodType.Wikipedia =>
                new WikipediaPodLoader( client, settings.WikipediaOptions ),
            _ => null
        };
}
