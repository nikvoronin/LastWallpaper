using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Models;
using LastWallpaper.Pods.Apod;
using LastWallpaper.Pods.Bing;
using LastWallpaper.Pods.Copernicus;
using LastWallpaper.Pods.Elementy;
using LastWallpaper.Pods.Nasa;
using LastWallpaper.Pods.Natgeotv;
using LastWallpaper.Pods.Wikimedia;
using System.Net.Http;

namespace LastWallpaper.Pods;

public static class PodsFactory
{
    public static IPotdLoader? Create(
        PodType podType,
        IResourceManager resourceManager,
        AppSettings settings )
        => podType switch {

            PodType.Bing =>
                new BingPodLoader(
                    new HttpClient(),
                    resourceManager,
                    settings.BingOptions ),

            // APOD open API
            PodType.Apod =>
                new ApodLoader(
                    new HttpClient(),
                    resourceManager,
                    settings.ApodOptions ),

            // APOD web-page parser
            PodType.ApodWeb =>
                new ApodWebLoader(
                    new HttpClient(),
                    resourceManager ),

            PodType.Wikipedia =>
                new WikipediaPodLoader(
                    new HttpClient() {
                        DefaultRequestHeaders = {
                            { "User-Agent", settings.UserAgent }
                        }
                    },
                    resourceManager ),

            PodType.Elementy =>
                new ElementyPodLoader(
                    new HttpClient(),
                    resourceManager,
                    new RssReader() ),

            PodType.Natgeotv =>
                new NatgeotvPodLoader(
                    new HttpClient(),
                    resourceManager ),

            PodType.Copernicus =>
                new CopernicusPodLoader(
                    new HttpClient(),
                    resourceManager ),

            PodType.Nasa =>
                new NasaPodLoader(
                    new HttpClient(),
                    resourceManager ),

            _ => null
        };
}
