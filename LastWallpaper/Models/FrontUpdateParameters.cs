namespace LastWallpaper.Models;

public class FrontUpdateParameters( bool hasNews, Imago? imago )
{
    public readonly bool HasNews = hasNews;
    public readonly Imago? Imago = imago;
}
