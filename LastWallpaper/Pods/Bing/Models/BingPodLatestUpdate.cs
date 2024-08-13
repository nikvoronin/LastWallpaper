using LastWallpaper.Models;

namespace LastWallpaper.Pods.Bing.Models;

public class BingPodLatestUpdate : PodLatestUpdate
{
    public required string LastImageUrl { get; init; }
    public string? Title { get; init; }
    public string? Copyright { get; init; }
}
