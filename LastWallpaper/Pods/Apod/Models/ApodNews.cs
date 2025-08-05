using LastWallpaper.Models;

namespace LastWallpaper.Pods.Apod.Models;

public sealed class ApodNews : PodNews
{
    public required ApodImageInfo Description { get; init; }
}
