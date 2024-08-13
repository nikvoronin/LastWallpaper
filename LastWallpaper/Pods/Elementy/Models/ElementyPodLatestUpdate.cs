using LastWallpaper.Models;
using LastWallpaper.Models.Rss;

namespace LastWallpaper.Pods.Elementy.Models;

public class ElementyPodLatestUpdate : PodLatestUpdate
{
    public required RssItem Item { get; init; }
}
