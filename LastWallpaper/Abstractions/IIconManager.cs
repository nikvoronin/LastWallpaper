using System.Drawing;

namespace LastWallpaper.Abstractions;

public interface IIconManager
{
    public Icon CreateIcon( string sourceImagePath );
    public bool DestroyIcon( Icon icon );
}
