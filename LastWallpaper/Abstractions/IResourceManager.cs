using System;

namespace LastWallpaper.Abstractions;

public interface IResourceManager
{
    /// <summary>
    /// Checks whether the image has been previously loaded and is known to the application.
    /// </summary>
    /// <param name="podName">Pod name.</param>
    /// <param name="updateTime">
    /// Update date-time of the picture-of-the-day.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if it has been previously loaded.
    /// </returns>
    public bool PotdAlreadyKnown(
        string podName,
        DateTimeOffset updateTime );
}
