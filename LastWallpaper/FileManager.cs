using FluentResults;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace LastWallpaper;

public static class FileManager
{
    public static void SaveCurrentImago( Imago imago ) =>
        File.WriteAllBytes(
            LastWallpaperFileName,
            JsonSerializer.SerializeToUtf8Bytes(
                imago, _intendedJsonSerializerOptions ) );

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

    public static AppSettings LoadAppSettings()
    {
        var appSettingsFileName =
            Path.Combine(
                GetAppFolder(),
                AppSettingsFileName );

        AppSettings? appSettings = null;

        if (File.Exists( appSettingsFileName )) {
            try {
                appSettings =
                    JsonSerializer.Deserialize<AppSettings>(
                        File.ReadAllText( appSettingsFileName ),
                        _jsonSerializerOptions );
            }
            catch { }
        }

        return appSettings ?? new();
    }

    public static string AlbumFolder {
        get {
            _albumPath ??=
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyPictures ),
                    Program.AppName,
                    DateTime.Now.Year.ToString() );

            if (!Directory.Exists( _albumPath ))
                Directory.CreateDirectory( _albumPath );

            return _albumPath;
        }
    }

    public static string CacheFolder {
        get {
            _cachePath ??=
                Path.Combine(
                    GetAppFolder(),
                    CacheFolderName );

            if (!Directory.Exists( _cachePath ))
                Directory.CreateDirectory( _cachePath );

            return _cachePath;
        }
    }

    private static string GetAppFolder() =>
        Path.GetDirectoryName( Application.ExecutablePath )!;

    private static string? _cachePath;
    private static string? _albumPath;

    private static readonly JsonSerializerOptions _intendedJsonSerializerOptions =
        new() {
            WriteIndented = true
        };

    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
            }
        };

    private const string CacheFolderName = "cache";
    private const string LastWallpaperFileName = "lastwallpaper.json";
    private const string AppSettingsFileName = "appsettings.json";
}
