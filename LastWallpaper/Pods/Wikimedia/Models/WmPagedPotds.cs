using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmPagedPotds
{
    [JsonPropertyName( "pageid" )]
    public required long PageId { get; init; }

    [JsonPropertyName( "title" )]
    public required string Title { get; init; }

    [JsonPropertyName( "missing" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Missing { get; init; }

    [JsonPropertyName( "images" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public IReadOnlyList<WmImageFilename>? Images { get; init; }

    [JsonPropertyName( "imageinfo" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public IReadOnlyList<WmImageInfo>? ImageInfos { get; init; }
}
