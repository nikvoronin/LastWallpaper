using LastWallpaper.Models;
using Microsoft.Win32;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LastWallpaper.Logic;

public static class WindowsRegistry
{
    public static void SetWallpaper(
        string imagePath,
        WallpaperStyle wallpaperStyle = WallpaperStyle.Default )
    {
        var windows = RuntimeInformation.IsOSPlatform( OSPlatform.Windows );
        if (!windows) return;

        if (!_wallpaperStyles.ContainsKey( wallpaperStyle ))
            wallpaperStyle = WallpaperStyle.Default;

        using var key =
            Registry.CurrentUser
            .OpenSubKey( @"Control Panel\Desktop", true );

        key?.SetValue(
            "WallpaperStyle",
            _wallpaperStyles[wallpaperStyle] );

        key?.SetValue(
            "TileWallpaper",
            wallpaperStyle == WallpaperStyle.Tile ? "1" : "0" );

        _ = SystemParametersInfo(
            SPI_SETDESKWALLPAPER,
            0,
            imagePath,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
    }

    static readonly FrozenDictionary<WallpaperStyle, string> _wallpaperStyles =
        new Dictionary<WallpaperStyle, string>() {
            { WallpaperStyle.Default, "10" }, // default is Fill
            { WallpaperStyle.Fill, "10" },
            { WallpaperStyle.Center, "0" },
            { WallpaperStyle.Tile, "0" }, // depend on "TileWallpaper" registry value
            { WallpaperStyle.Stretch, "2" },
            { WallpaperStyle.Span, "22" },
            { WallpaperStyle.Fit, "6" },
        }.ToFrozenDictionary();

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
    static extern int SystemParametersInfo( int uAction, int uParam, string lpvParam, int fuWinIni );
}
