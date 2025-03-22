using LastWallpaper.Abstractions;
using System;

namespace LastWallpaper.Models;

/// <summary>
/// Base value set of pod latest update.
/// </summary>
public class PodNews : IPotdNews
{
    public required PodType PodType { get; init; }
    public required DateTimeOffset PubDate { get; init; }
}
