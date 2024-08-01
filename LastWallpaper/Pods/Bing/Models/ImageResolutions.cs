namespace LastWallpaper.Pods.Bing.Models;

public static class ImageResolutions
{
    public static string GetValue( ImageResolution resolutionName )
        => resolutionName switch {
            ImageResolution.FullHD
                or ImageResolution.FHD => FullHD,

            ImageResolution.HD => HD,

            ImageResolution.UltraHD
                or ImageResolution.UHD
                or _ => UltraHD
        };

    public const string HD = "1280x720";
    public const string FullHD = "1920x1080";
    public const string UltraHD = "UHD";
}
