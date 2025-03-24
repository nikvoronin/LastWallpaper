using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Natgeotv.Fetchers;
using System.Net.Http;

namespace LastWallpaper.Pods.Natgeotv;

public sealed class NatgeotvPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<HtmlPodNews>(
        PodType.Natgeotv,
        new HtmlNewsFetcher<HtmlPodNews>(
            httpClient,
            new( "https://www.natgeotv.com/ca/photo-of-the-day" ),
            new NatgeotvNewsExtractor() ),
        new HttpPotdFetcher<HtmlPodNews>(
            httpClient,
            new NatgeotvDescriptionFetcher(),
            resourceManager ),
        resourceManager )
{ }
