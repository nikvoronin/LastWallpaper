using LastWallpaper.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
#if !DEBUG
using System.Threading.Tasks;
#endif
using System.Windows.Forms;

namespace LastWallpaper;

public sealed class UpdateUiHandler(
    SynchronizationContext uiContext,
    NotifyIcon notifyIconCtrl )
    : IUpdateHandler, IDisposable
{
    private void UpdateInternal(
        Imago imago,
        bool restoreUi = false )
    {
        if (_currentIconHandle != 0) {
            IconManager.DestroyIcon( _currentIconHandle );
            _currentIconHandle = 0;
        }

        _notifyIconCtrl.Icon = IconManager.CreateIcon( imago.Filename );
        _currentIconHandle = _notifyIconCtrl.Icon.Handle;
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

    public void Dispose()
    {
        if (_currentIconHandle != 0)
            IconManager.DestroyIcon( _currentIconHandle );
    }

    private nint _currentIconHandle = 0;
    private readonly NotifyIcon _notifyIconCtrl = notifyIconCtrl;
    private readonly SynchronizationContext _uiContext = uiContext;
}
