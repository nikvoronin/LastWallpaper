using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Models;
using LastWallpaper.Pods.Astrobin;
using LastWallpaper.Pods.Bing;
using LastWallpaper.Pods.Elementy;
// TODO: return after refactoring
//using LastWallpaper.Pods.Copernicus;
//using LastWallpaper.Pods.Nasa;
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

            // TODO: return after refactoring
            //PodType.Apod =>
            //    new NasaApodLoader(
            //        client,
            //        resourceManager,
            //        settings.ApodOptions ),

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

            PodType.Astrobin =>
                new AstrobinPodLoader(
                    new HttpClient(),
                    resourceManager ),

            PodType.Natgeotv =>
                new NatgeotvPodLoader(
                    new HttpClient(),
                    resourceManager ),

            //PodType.Copernicus =>
            //    new CopernicusPodLoader(
            //        client,
            //        resourceManager ),

            //PodType.Nasa =>
            //    new NasaPodLoader(
            //        client,
            //        resourceManager ),

            _ => null
        };
}
