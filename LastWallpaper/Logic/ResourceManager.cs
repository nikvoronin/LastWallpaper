﻿using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Text.Json;

namespace LastWallpaper.Logic;

public class ResourceManager(
    string appFolder,
    string albumFolder,
    string cacheFolder )
    : IResourceManager
{
    public string AlbumFolder {
        get {
            var separateByYear = true; // TODO: STUB extract to settings

            var albumFolder =
                separateByYear
                    ? Path.Combine(
                        _albumFolderBase,
                        DateTime.Now.Year.ToString() )
                    : _albumFolderBase;

            if (!Directory.Exists( albumFolder ))
                Directory.CreateDirectory( albumFolder );

            return albumFolder;
        }
    }

    public string CreateAlbumFilename(
        string podName,
        DateTimeOffset potdCreationTime )
        => Path.Combine(
            AlbumFolder,
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

    public string SystemDesktopWallpaperFilename =>
        Path.Combine(
            Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
            "Microsoft/Windows/Themes/TranscodedWallpaper" );

    private static readonly JsonSerializerOptions _intendedJsonSerializerOptions =
        new() { WriteIndented = true };

    private readonly string _appFolder = appFolder;
    private readonly string _albumFolderBase = albumFolder;
    private readonly string _cacheFolder = cacheFolder;
}
