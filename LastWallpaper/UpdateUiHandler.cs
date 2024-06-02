using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LastWallpaper;

public interface IUpdateHandler
{
    void HandleUpdate( Imago imago );
}

public sealed class UpdateUiHandler(
    SynchronizationContext uiContext,
    NotifyIcon notifyIconCtrl )
    : IUpdateHandler
{
    public void HandleUpdate( Imago imago )
    {
#if !DEBUG
        Task.Run( () => WindowsRegistry.SetWallpaper( imago.Filename ) );
#endif
        _notifyIconCtrl.Icon = IconManager.CreateIcon( imago.Filename );

        _uiContext?.Post( _ =>
            ToastNotifications.ShowToast(
                imago.Filename,
                imago.Title,
                imago.Copyright ),
            null );
    }

    private readonly NotifyIcon _notifyIconCtrl = notifyIconCtrl;
    private readonly SynchronizationContext _uiContext = uiContext;
}
