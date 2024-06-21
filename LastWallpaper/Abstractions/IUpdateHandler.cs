using LastWallpaper.Models;

namespace LastWallpaper.Abstractions;

public interface IUpdateHandler
{
    void HandleUpdate(Imago imago);
}