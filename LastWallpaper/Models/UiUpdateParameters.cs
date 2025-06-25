using System;

namespace LastWallpaper.Models;

public class UiUpdateParameters(
    UiUpdateTargets updateTargets,
    PodUpdateResult podUpdateResult )
{
    public readonly UiUpdateTargets UpdateTargets = updateTargets;
    public readonly PodUpdateResult UpdateResult =
        podUpdateResult ?? throw new ArgumentNullException( nameof( podUpdateResult ) );
}
