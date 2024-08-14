using HtmlAgilityPack;
using LastWallpaper.Models;

namespace LastWallpaper.Pods.Astrobin.Models;

public sealed class AbinPodNews : PodNews
{
    public required HtmlDocument Document { get; init; }
    public required AbinIotdDescription IotdDescription { get; init; }
}
