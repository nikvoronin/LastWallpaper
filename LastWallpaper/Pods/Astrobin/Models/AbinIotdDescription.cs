using System;

namespace LastWallpaper.Pods.Astrobin.Models;

public class AbinIotdDescription
{
    public required string HdPageUrl { get; init; }
    public required string Author { get; init; }
    public required string Title { get; init; }
    public required DateTime PubDate { get; init; }
}
