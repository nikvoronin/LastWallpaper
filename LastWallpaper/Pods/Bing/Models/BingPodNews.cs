using LastWallpaper.Models;

namespace LastWallpaper.Pods.Bing.Models;

public sealed class BingPodNews : PodNews
{
    public required string LastImageUrl { get; init; }
    public required string Title { get; init; }
    public required string Copyright { get; init; }
}
