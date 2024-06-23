using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Threading;
#if !DEBUG
using System.Threading.Tasks;
#endif
using System.Windows.Forms;

namespace LastWallpaper.Handlers;

public sealed class FrontUpdateHandler(
    SynchronizationContext uiContext,
    NotifyIcon notifyIconCtrl )
    : IParameterizedUpdateHandler<Imago>, IDisposable
{
    private void UpdateInternal(
        Imago imago,
        bool restoreUi = false )
    {
        if (_currentIconHandle != 0) {
            IconManager.DestroyIcon( _currentIconHandle );
            _currentIconHandle = 0;
        }

        try {
            _notifyIconCtrl.Icon = IconManager.CreateIcon( imago.Filename );
            _currentIconHandle = _notifyIconCtrl.Icon.Handle;
        }
        catch (FileNotFoundException) { }
        
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

    public void RestoreUi( Imago imago ) =>
        UpdateInternal( imago, restoreUi: true );

    public void HandleUpdate( Imago imago, CancellationToken _ ) =>
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
