using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Bing.Models;

public class BingHpImages
{
    [JsonPropertyName( "images" )]
    public IReadOnlyList<ImageInfo>? Images { get; set; }
}
