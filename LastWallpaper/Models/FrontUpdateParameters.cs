namespace LastWallpaper.Models;

public class FrontUpdateParameters(
    bool updateWallpaper,
    PodUpdateResult podUpdateResult )
{
    public readonly bool ShouldUpdateWallpaper = updateWallpaper;
    public readonly PodUpdateResult UpdateResult = podUpdateResult;
}
