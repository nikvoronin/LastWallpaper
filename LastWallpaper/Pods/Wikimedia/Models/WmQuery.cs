using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Wikimedia.Models;

public sealed class WmQuery
{
    [JsonPropertyName( "pages" )]
    public required IReadOnlyList<WmPagedPotds> Pages { get; init; }
}
