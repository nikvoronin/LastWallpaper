using LastWallpaper.Models;
using LastWallpaper.Models.Rss;

namespace LastWallpaper.Pods.Elementy.Models;

public sealed class ElementyPodNews : PodNews
{
    public required RssItem Item { get; init; }
}
