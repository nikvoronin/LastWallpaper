using System;

namespace LastWallpaper.Models;

/// <summary>
/// Parameter set of pod update results.
/// </summary>
public record PodUpdateResult
{
    public required string PodName { get; init; }
    public required string Filename { get; init; }
    public required DateTime Created { get; init; }
    public string? Copyright { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
}
