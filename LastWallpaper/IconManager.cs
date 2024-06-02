using System.Drawing;
using System.Linq;

namespace LastWallpaper;

public static class IconManager
{
    public static Icon CreateIcon( string imagePath )
    {
        var src = new Bitmap( imagePath );
        var dst =
            new Bitmap(
                DefaultTrayIconSize.Width,
                DefaultTrayIconSize.Height );

        // TODO: to options, ability to draw border around the tray icon
        var g = Graphics.FromImage( dst );
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        g.DrawImage( src, 0, 0, dst.Width, dst.Height );

        var pen = PenBy( FindBrightestColor( dst ) );
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

                if (bright > maxBright) {
                    maxColor = pixel;
                    maxBright = bright;

                    if (maxBright == 1f) goto WhiteColorFound;
                }
            }
        }
    WhiteColorFound:;

        return maxColor;
    }

    private static Pen PenBy( Color color ) => new( color );

    private static Size DefaultTrayIconSize = new( 20, 20 );
}
