using LastWallpaper.Models;
using System;

namespace LastWallpaper.Abstractions;

public interface IPotdNews
{
    PodType PodType { get; }
    DateTimeOffset PubDate { get; }
}
