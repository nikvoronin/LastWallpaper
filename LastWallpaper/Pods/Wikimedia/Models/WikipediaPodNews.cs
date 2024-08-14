using LastWallpaper.Models;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WikipediaPodNews : PodNews
{
    public required WmResponse Response { get; init; }
}
