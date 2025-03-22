using LastWallpaper.Models;

namespace LastWallpaper.Logic;

public static class PodTypeExtensions
{
    public static string ToPodName( this PodType podType ) =>
        podType.ToString().ToLower();
}
