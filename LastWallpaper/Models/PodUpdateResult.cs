using System;

namespace LastWallpaper.Models;

/// <summary>
/// Parameter set of pod update results.
/// </summary>
public record PodUpdateResult
{
    public required PodType PodType { get; init; }
    public required string Filename { get; init; }
    public required DateTimeOffset Created { get; init; }
    public bool CopyToAlbum { get; init; } = true;
    public string? Copyright { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
}
