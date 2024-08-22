using LastWallpaper.Models;

namespace LastWallpaper.Pods.Natgeotv.Models;

public sealed class NatgeotvPodNews : PodNews
{
    public required NatgeotvPotdDescription PodDescription { get; init; }
}
