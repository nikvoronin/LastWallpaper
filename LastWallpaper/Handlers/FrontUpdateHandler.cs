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
    SynchronizationContext _uiContext,
    NotifyIcon _notifyIconCtrl,
    AppSettings settings )
    : IParameterizedUpdateHandler<FrontUpdateParameters>, IDisposable
{
    public void HandleUpdate(
        FrontUpdateParameters updateParameters,
        CancellationToken ct )
    {
        var imago =
            updateParameters.HasNews ? updateParameters.Imago
            : FileManager.LoadLastImago().ValueOrDefault;

        if (imago is null) return;

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

        if (updateParameters.HasNews) {
#if !DEBUG
            Task.Run( () =>
                WindowsRegistry.SetWallpaper( imago.Filename ) );
#endif
            _uiContext?.Post( _ =>
                ToastNotifications.ShowToast(
                    imago.Filename,
                    imago.Title,
                    imago.Copyright,
                    settings.ToastExpireIn ),
                null );

            FileManager.SaveCurrentImago( imago );
        }
    }

    public void Dispose()
    {
        if (_currentIconHandle != 0)
            IconManager.DestroyIcon( _currentIconHandle );
    }

    private nint _currentIconHandle = 0;
}
