using LastWallpaper.Abstractions;
using System;
using System.IO;

namespace LastWallpaper;

public class ResourceManager : IResourceManager
{
    public string CreateAlbumFilename(
        string podName,
        DateTimeOffset potdCreationTime )
        => Path.Combine(
            FileManager.AlbumFolder,
            $"{podName}{potdCreationTime:yyyyMMdd}.jpeg" );

    public FileStream CreateTemporaryFileStream() =>
        new(
            Path.Combine(
                FileManager.CacheFolder,
                Guid.NewGuid().ToString() ),
            FileMode.Create );

    public bool PotdExists(
        string podName,
        DateTimeOffset updateTime )
        => File.Exists(
            CreateAlbumFilename(
                podName, updateTime ) );
}
