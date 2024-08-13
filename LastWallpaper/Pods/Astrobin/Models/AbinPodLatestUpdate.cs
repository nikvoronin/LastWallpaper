using HtmlAgilityPack;
using LastWallpaper.Models;

namespace LastWallpaper.Pods.Astrobin.Models;

public class AbinPodLatestUpdate : PodLatestUpdate
{
    public required HtmlDocument Document { get; init; }
    public required AbinIotdDescription IotdDescription { get; init; }
}
