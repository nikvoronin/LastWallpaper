using System;

namespace LastWallpaper.Models;

/// <summary>
/// The final description of the picture of the day.
/// </summary>
public class PotdDescription : PodNews
{
    public required Uri Url { get; init; }
    public required string Title { get; init; }
    public required string Copyright { get; init; }
    public string? Description { get; init; }
}
