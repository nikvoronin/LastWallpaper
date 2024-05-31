using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace LastWallpaper;

public static class WindowsRegistry
{
    public static void SetWallpaper(
        string imagePath,
        Position position = Position.Default )
    {
        var windows = RuntimeInformation.IsOSPlatform( OSPlatform.Windows );
        if (!windows) return;

        using var key = Registry.CurrentUser.OpenSubKey( @"Control Panel\Desktop", true );

        key?.SetValue( "WallpaperStyle", position.ToStyleValue() );
        key?.SetValue( "TileWallpaper", position.ToTileValue() );

        _ = SystemParametersInfo(
            SPI_SETDESKWALLPAPER,
            0,
            imagePath,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
    }

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
    static extern int SystemParametersInfo( int uAction, int uParam, string lpvParam, int fuWinIni );
}

public enum Position { Default, Center, Tile, Stretch, Span, Fit, Fill }

file static class Extension
{
    public static string ToStyleValue( this Position position ) =>
        position switch {
            Position.Center
                or Position.Tile => "0",
            Position.Stretch => "2",
            Position.Span => "22",
            Position.Fit => "6",
            _ => "10" // Position.Fill by default
        };

    public static string ToTileValue( this Position position ) =>
        position == Position.Tile ? "1" : "0";
}