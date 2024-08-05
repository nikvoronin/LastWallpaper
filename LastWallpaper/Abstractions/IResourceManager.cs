using System;
using System.IO;

namespace LastWallpaper.Abstractions;

public interface IResourceManager
{
    /// <summary>
    /// Checks whether the image has been previously loaded and is known to the application.
    /// </summary>
    /// <param name="podName">Pod name.</param>
    /// <param name="potdCreationTime">
    /// Date-time when the picture-of-the-day was created.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if it has been previously loaded.
    /// </returns>
    public bool PotdExists(
        string podName,
        DateTimeOffset potdCreationTime );

    public FileStream CreateTemporaryFileStream();
}
