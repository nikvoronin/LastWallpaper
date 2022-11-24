using Microsoft.Win32;
using System.Runtime.InteropServices;
using static LastWallpaper.WindowsRegistry;

namespace LastWallpaper
{
    public sealed class WindowsRegistry
    {
        public static void SetWallpaper(
            string imagePath,
            Position p = Position.Fill )
        {
            var windows = RuntimeInformation.IsOSPlatform( OSPlatform.Windows );
            if ( !windows ) return;

            var key = Registry.CurrentUser.OpenSubKey( @"Control Panel\Desktop", true );

            key?.SetValue( @"WallpaperStyle", p.ToStyleValue() );
            key?.SetValue( @"TileWallpaper", p.ToTileValue() );

            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                imagePath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
        }

        public enum Position { Center, Tile, Stretch, Span, Fit, Fill }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
        static extern int SystemParametersInfo( int uAction, int uParam, string lpvParam, int fuWinIni );
    }

    file static class Extension
    {
        public static string ToStyleValue( this Position p )
            => p switch {
                Position.Center => "0",
                Position.Tile => "0",
                Position.Stretch => "2",
                Position.Span => "22",
                Position.Fit => "6",
                Position.Fill or _ => "10"
            };

        public static string ToTileValue( this Position p )
            => p == Position.Tile ? "1" : "0";
    }
}
