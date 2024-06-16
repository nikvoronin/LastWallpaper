using LastWallpaper.Models;
#if !DEBUG
using System.Threading.Tasks;
#endif

namespace LastWallpaper;

public interface IUpdateHandler
{
    void HandleUpdate( Imago imago );
}
