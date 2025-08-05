using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Apod.Fetchers;
using LastWallpaper.Pods.Apod.Models;
using System.Net.Http;

namespace LastWallpaper.Pods.Apod;

public sealed class ApodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    ApodSettings settings )
    : PodLoader<ApodNews>(
        PodType.Apod,
        new ApodNewsFetcher(
            httpClient,
            settings ),
        new HttpPotdFetcher<ApodNews>(
            httpClient,
            new ApodDescriptionFetcher(),
            resourceManager ),
        resourceManager )
{ }

