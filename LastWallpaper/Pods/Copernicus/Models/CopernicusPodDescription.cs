using System;

namespace LastWallpaper.Pods.Copernicus.Models;

public class CopernicusPodDescription
{
    public required string Url { get; init; }
    public required string Author { get; init; }
    public required string Title { get; init; }
    public required DateTime PubDate { get; init; }
}
