using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Apod.Fetchers;
using System.Net.Http;

namespace LastWallpaper.Pods.Apod;

public sealed class ApodWebLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<HtmlPodNews>(
        PodType.ApodWeb,
        new HtmlNewsFetcher<HtmlPodNews>(
            httpClient,
            new( ApodPixUrl ),
            new ApodWebNewsExtractor(
                new( ApodBaseUrl ) ) ),
        new HttpPotdFetcher<HtmlPodNews>(
            httpClient,
            new HtmlDescriptionFetcher(
                new() { UseCopyrightSign = true } ),
            resourceManager ),
        resourceManager )
{
    private const string ApodBaseUrl = "https://apod.nasa.gov/apod";
    private const string ApodPixUrl = ApodBaseUrl + "/astropix.html";
}