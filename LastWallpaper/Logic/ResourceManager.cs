using LastWallpaper.Abstractions;
using System;
using System.IO;

namespace LastWallpaper;

public class ResourceManager : IResourceManager
{
    public bool PotdAlreadyKnown( string podName, DateTimeOffset updateTime )
    {
        var filename =
            Path.Combine(
                FileManager.AlbumFolder,
                $"{podName}{updateTime:yyyyMMdd}.jpeg" );

        return File.Exists( filename );
    }
}
