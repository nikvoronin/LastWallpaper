using System;

namespace LastWallpaper.Models;

/// <summary>
/// Description of common image of the day.
/// </summary>
public class Imago
{
    public required string Filename { get; init; }
    public required DateTime Created { get; init; }
    public string? Copyright { get; init; }
    public string? Title { get; init; }
}
