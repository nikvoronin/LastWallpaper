using LastWallpaper.Abstractions;
using System;
using System.IO;

namespace LastWallpaper;

public class ResourceManager : IResourceManager
{
    public string CreateTemporaryCacheFilename() =>
        Path.Combine(
            FileManager.CacheFolder,
            Guid.NewGuid().ToString() );

    public bool IsPotdAlreadyKnown( string podName, DateTimeOffset updateTime )
    {
        var filename =
            Path.Combine(
                FileManager.AlbumFolder,
                $"{podName}{updateTime:yyyyMMdd}.jpeg" );

        return File.Exists( filename );
    }
}
