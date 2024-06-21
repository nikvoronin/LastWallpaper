using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace LastWallpaper;

public static class IconManager
{
    // TODO: add strategies - thumbnail, k-tile, ...
    public static Icon CreateIcon( string imagePath )
    {
        using var src = new Bitmap( imagePath );
        using var dst =
            new Bitmap(
                DefaultTrayIconSize.Width,
                DefaultTrayIconSize.Height );

        using var g = Graphics.FromImage( dst );
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        g.DrawImage( src, 0, 0, dst.Width, dst.Height );

        using var pen = WidePenBy( FindBrightestColor( dst ), 24 );
        g.DrawRectangle( pen, 0, 0, dst.Width - 1, dst.Height - 1 );

        return Icon.FromHandle( dst.GetHicon() );
    }

    private static Color FindBrightestColor( Bitmap bitmap )
    {
        Color maxColor = Color.Black;
        var maxBright = 0f;

        foreach (var y in Enumerable.Range( 0, bitmap.Height )) {
            foreach (var x in Enumerable.Range( 0, bitmap.Width )) {
                var pixel = bitmap.GetPixel( x, y );
                var bright = pixel.GetBrightness();

                if (bright >= 1f) return Color.White;

                if (bright > maxBright) {
                    maxColor = pixel;
                    maxBright = bright;
                }
            }
        }

        return maxColor;
    }

    private static Pen PenBy( Color color ) => new( color );
    private static Pen WidePenBy( Color color, int width ) => new( color, width );

    private static Size DefaultTrayIconSize = new( 256, 256 );

    [DllImport( "user32.dll", CharSet = CharSet.Auto )]
    public extern static bool DestroyIcon( IntPtr handle );
}
