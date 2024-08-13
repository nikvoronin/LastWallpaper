using LastWallpaper.Models;

namespace LastWallpaper.Pods.Nasa.Models;

public class NasaApodLatestUpdate : PodLatestUpdate
{
    public required ImageInfo Description { get; init; }
}
