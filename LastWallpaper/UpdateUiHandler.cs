using LastWallpaper.Models;
using System.Diagnostics;
using System.Threading;

namespace LastWallpaper;

public interface IUpdateHandler
{
    void HandleUpdate( Imago imago );
}

public sealed class UpdateUiHandler : IUpdateHandler
{
    public UpdateUiHandler( SynchronizationContext uiContext )
    {
        Debug.Assert( uiContext is not null );

        _uiContext = uiContext;
    }

    public void HandleUpdate( Imago imago )
    {
        _uiContext?.Post( _ =>
            ToastNotifications.ShowToast(
                imago.Filename,
                imago.Title,
                imago.Copyright ),
            null);
    }

    private readonly SynchronizationContext _uiContext;
}
