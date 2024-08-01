using LastWallpaper.Abstractions;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Bing.Models;

public class BingSettings : IPotdLoaderSettings
{
    [JsonPropertyName("resolution")]
    public ImageResolution Resolution { get; init; } =
        ImageResolution.UltraHD;
}
