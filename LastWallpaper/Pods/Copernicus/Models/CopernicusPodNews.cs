using LastWallpaper.Models;

namespace LastWallpaper.Pods.Copernicus.Models;

public sealed class CopernicusPodNews : PodNews
{
    public required CopernicusPodDescription PodDescription { get; init; }
}
