using FluentResults;
using LastWallpaper.Models;
using System;
using System.IO;

namespace LastWallpaper.Abstractions;

public interface IResourceManager
{
    /// <summary>
    /// Full path to the POD album folder.
    /// </summary>
    string AlbumFolder { get; }

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
    bool PotdExists(
        string podName,
        DateTimeOffset potdCreationTime );

    /// <summary>
    /// Creates filename based on pod name and creation time.
    /// </summary>
    /// <param name="podName">Name of the pod</param>
    /// <param name="potdCreationTime">Potd creation or update date-time.</param>
    /// <returns>Filename with full path inside album folder.</returns>
    string CreateAlbumFilename(
        string podName,
        DateTimeOffset potdCreationTime );

    /// <summary>
    /// Open file stream to temporary cache file.
    /// </summary>
    /// <returns><see cref="FileStream"/> to temporary cache file.</returns>
    FileStream CreateTemporaryFileStream();

    /// <summary>
    /// Restore description about the last known wallpaper.
    /// </summary>
    /// <returns>
    /// Description of the last known wallpaper.
    /// </returns>
    Result<PodUpdateResult> RestoreLastWallpaper();

    /// <summary>
    /// Store description about given wallpaper.
    /// </summary>
    /// <param name="podUpdateResult">
    /// Wallpaper description.
    /// </param>
    void RememberLastWallpaper( PodUpdateResult podUpdateResult );

    /// <summary>
    /// Full filename (with full path) of the current system desktop wallpaper.
    /// </summary>
    string SystemDesktopWallpaperFilename { get; }
}
