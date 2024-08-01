using LastWallpaper.Abstractions;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LastWallpaper.Logic;

public abstract class IconManager : IIconManager
{
    public abstract Icon CreateIcon( string sourceImagePath );

    public bool DestroyIcon( Icon icon ) => DestroyIcon( icon.Handle );

    [DllImport( "user32.dll", CharSet = CharSet.Auto )]
    private extern static bool DestroyIcon( nint handle );

    protected static Size DefaultTrayIconSize = new( 256, 256 );
}
