using LastWallpaper.Models;

namespace LastWallpaper.Pods.Nasa.Models;

public sealed class NasaApodNews : PodNews
{
    public required ImageInfo Description { get; init; }
}
