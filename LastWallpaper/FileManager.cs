using FluentResults;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace LastWallpaper;

public static class FileManager
{
    public static void SaveCurrentImago( Imago imago ) =>
        File.WriteAllBytes(
            LastWallpaperFileName,
            JsonSerializer.SerializeToUtf8Bytes(
                imago,
                new JsonSerializerOptions { WriteIndented = true } ) );

    public static Result<Imago> LoadLastImago()
    {
        var lastWallpaperFilename =
            Path.Combine(
                GetAppFolder(),
                LastWallpaperFileName );

        if (File.Exists( lastWallpaperFilename )) {
            try {
                return Result.Ok(
                    JsonSerializer.Deserialize<Imago>(
                        File.ReadAllText( lastWallpaperFilename ) )
                    ?? throw new InvalidDataException() );
            }
            catch { }
        }

        return Result.Fail( "The last wallpaper was not found" );
    }

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
                        GetAppFolder(),
                        CacheFolderName );

                if (!Directory.Exists( _cachePath ))
                    Directory.CreateDirectory( _cachePath );
            }

            return _cachePath;
        }
    }

    public static string GetAppFolder() =>
        Path.GetDirectoryName( Application.ExecutablePath )!;

    public static void ClearCache()
    {
        if (_cachePath is not null)
            Directory.Delete( _cachePath, true );
    }

    private static string? _cachePath;
    private static string? _albumPath;

    private const string CacheFolderName = "cache";
    private const string LastWallpaperFileName = "last-wallpaper.json";
}
