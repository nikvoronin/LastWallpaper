using LastWallpaper.Models;

namespace LastWallpaper.Pods.Bing.Models;

public sealed class BingPodNews : PodNews
{
    public required string LastImageUrl { get; init; }
    public string? Title { get; init; }
    public string? Copyright { get; init; }
}
