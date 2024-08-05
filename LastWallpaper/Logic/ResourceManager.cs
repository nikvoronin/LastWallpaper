using LastWallpaper.Abstractions;
using System;
using System.IO;

namespace LastWallpaper;

public class ResourceManager : IResourceManager
{
    public FileStream CreateTemporaryFileStream() =>
        new(
            Path.Combine(
                FileManager.CacheFolder,
                Guid.NewGuid().ToString() ),
            FileMode.Create );

    public bool PotdExists( string podName, DateTimeOffset updateTime ) =>
        File.Exists(
            Path.Combine(
                FileManager.AlbumFolder,
                $"{podName}{updateTime:yyyyMMdd}.jpeg" ) );
}
