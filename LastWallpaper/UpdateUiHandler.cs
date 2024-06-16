using LastWallpaper.Models;
using System.Threading;
#if !DEBUG
using System.Threading.Tasks;
#endif
using System.Windows.Forms;

namespace LastWallpaper;

public sealed class UpdateUiHandler(
    SynchronizationContext uiContext,
    NotifyIcon notifyIconCtrl )
    : IUpdateHandler
{
    private void UpdateInternal(
        Imago imago,
        bool restoreUi = false )
    {
        _notifyIconCtrl.Icon = IconManager.CreateIcon( imago.Filename );
        _notifyIconCtrl.Text =
            $"{Program.AppName} #{imago.PodName}\n{imago.Created:D} {imago.Created:t}";

        if (!restoreUi) {
#if !DEBUG
            Task.Run( () =>
                WindowsRegistry.SetWallpaper( imago.Filename ) );
#endif
            _uiContext?.Post( _ =>
                ToastNotifications.ShowToast(
                    imago.Filename,
                    imago.Title,
                    imago.Copyright ),
                null );

            FileManager.SaveCurrentImago( imago );
        }
    }

    public void InitialUpdate( Imago imago ) =>
        UpdateInternal( imago, restoreUi: true );

    public void HandleUpdate( Imago imago ) =>
        UpdateInternal( imago );

    private readonly NotifyIcon _notifyIconCtrl = notifyIconCtrl;
    private readonly SynchronizationContext _uiContext = uiContext;
}
