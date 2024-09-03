using LastWallpaper.Models;
using System;

namespace LastWallpaper.Pods.Nasa.Models;

public class NasaPodNews : PodNews
{
    public bool HasCredits =>
        !string.IsNullOrWhiteSpace( CreditsPageUrl );

    public required string Title { get; init; }
    public required Uri ImageUrl { get; init; }
    public string? Description { get; init; }
    public string? CreditsPageUrl { get; init; }
}
