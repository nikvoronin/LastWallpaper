using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Text.Json;

namespace LastWallpaper;

public class ResourceManager(
    string appFolder,
    string albumFolder,
    string cacheFolder )
    : IResourceManager
{
    public string CreateAlbumFilename(
        string podName,
        DateTimeOffset potdCreationTime )
        => Path.Combine(
            _albumFolder,
            $"{podName}{potdCreationTime:yyyyMMdd}.jpeg" );

    public FileStream CreateTemporaryFileStream() =>
        new(
            Path.Combine(
                _cacheFolder,
                Guid.NewGuid().ToString() ),
            FileMode.Create );

    public bool PotdExists(
        string podName,
        DateTimeOffset updateTime )
        => File.Exists(
            CreateAlbumFilename(
                podName, updateTime ) );

    public void RememberLastWallpaper( PodUpdateResult imago )
    {
        var lastWallpaperFilename =
            Path.Combine( _appFolder, Program.LastWallpaperFileName );

        File.WriteAllBytes(
            lastWallpaperFilename,
            JsonSerializer.SerializeToUtf8Bytes(
                imago, _intendedJsonSerializerOptions ) );
    }

    public Result<PodUpdateResult> RestoreLastWallpaper()
    {
        var lastWallpaperFilename =
            Path.Combine( _appFolder, Program.LastWallpaperFileName );

        if (File.Exists( lastWallpaperFilename )) {
            try {
                return Result.Ok(
                    JsonSerializer.Deserialize<PodUpdateResult>(
                        File.ReadAllText( lastWallpaperFilename ) )
                    ?? throw new InvalidDataException() );
            }
            catch { }
        }

        return Result.Fail( "The last wallpaper was not found." );
    }

    private static readonly JsonSerializerOptions _intendedJsonSerializerOptions =
        new() { WriteIndented = true };

    private readonly string _appFolder = appFolder;
    private readonly string _albumFolder = albumFolder;
    private readonly string _cacheFolder = cacheFolder;
}
