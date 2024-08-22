using System;

namespace LastWallpaper.Pods.Natgeotv.Models;

public class NatgeotvPotdDescription
{
    public required string Url { get; init; }
    public required string Author { get; init; }
    public required string Title { get; init; }
    public required DateTime PubDate { get; init; }
}
