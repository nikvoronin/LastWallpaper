using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Nasa.Fetchers;
using LastWallpaper.Pods.Nasa.Models;
using System.Net.Http;

namespace LastWallpaper.Pods.Nasa;

public sealed class NasaPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<NasaPodNews>(
        PodType.Nasa,
        new HtmlNewsFetcher<NasaPodNews>(
            httpClient,
            new( "https://www.nasa.gov/image-of-the-day/" ),
            new NasaNewsExtractor() ),
        new HttpPotdFetcher<NasaPodNews>(
            httpClient,
            new NasaDescriptionFetcher( httpClient ),
            resourceManager ),
        resourceManager )
{ }