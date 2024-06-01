using System;
using System.IO;
using System.Windows.Forms;

namespace LastWallpaper;

public static class FileManager
{
    public static string AlbumFolder {
        get {
            if (_albumPath is null) {
                _albumPath =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyPictures ),
                        Program.AppName,
                        DateTime.Now.Year.ToString() );

                if (!Directory.Exists( _albumPath ))
                    Directory.CreateDirectory( _albumPath );
            }

            return _albumPath;
        }
    }

    public static string CacheFolder {
        get {
            if (_cachePath is null) {
                _cachePath =
                    Path.Combine(
                        Path.GetDirectoryName( Application.ExecutablePath )!,
                        CacheFolderName );

                if (!Directory.Exists( _cachePath ))
                    Directory.CreateDirectory( _cachePath );
            }

            return _cachePath;
        }
    }

    public static void ClearCache()
    {
        if (_cachePath is not null)
            Directory.Delete( _cachePath, true );
    }

    private static string? _cachePath;
    private static string? _albumPath;

    private const string CacheFolderName = "cache";
}
