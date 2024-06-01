using LastWallpaper.Models;
using System.Threading;

namespace LastWallpaper;

public interface IUpdateHandler
{
    void HandleUpdate( Imago imago );
}

public sealed class UpdateUiHandler( SynchronizationContext uiContext )
    : IUpdateHandler
{
    public void HandleUpdate( Imago imago )
    {
        _uiContext?.Post( _ =>
            ToastNotifications.ShowToast(
                imago.Filename,
                imago.Title,
                imago.Copyright ),
            null );
    }

    private readonly SynchronizationContext _uiContext = uiContext;
}
