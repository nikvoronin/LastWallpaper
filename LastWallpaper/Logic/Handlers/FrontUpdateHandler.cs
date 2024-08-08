using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
#if !DEBUG
using System.Threading.Tasks;
#endif
using System.Windows.Forms;

namespace LastWallpaper.Logic.Handlers;

public sealed class FrontUpdateHandler(
    SynchronizationContext uiContext,
    NotifyIcon notifyIconCtrl,
    IIconManager iconManager,
    AppSettings settings )
    : IParameterizedUpdateHandler<FrontUpdateParameters>
    , IDisposable
{
    public void HandleUpdate(
        FrontUpdateParameters updateParameters,
        CancellationToken ct )
    {
        var imago = updateParameters.UpdateResult;
        if (imago is null) return;

        if (_currentIcon is not null)
            _iconManager.DestroyIcon( _currentIcon );

        try {
            _notifyIconCtrl.Icon = _iconManager.CreateIcon( imago.Filename );
            _currentIcon = _notifyIconCtrl.Icon;
        }
        catch (FileNotFoundException) { }

        _notifyIconCtrl.Text =
            $"{Program.AppName} #{imago.PodName}\n{imago.Created:D} {imago.Created:t}";

        if (updateParameters.ShouldUpdateWallpaper) {
#if !DEBUG
            Task.Run( () =>
                WindowsRegistry.SetWallpaper(
                    imago.Filename,
                    _settings.WallpaperFit ),
                    ct );
#endif
            _uiContext?.Post( _ =>
                ToastNotifications.ShowToast(
                    imago.Filename,
                    imago.Title,
                    imago.Copyright,
                    _settings.ToastExpireIn ),
                null );
        }
    }

    public void Dispose()
    {
        if (_currentIcon is not null)
            _iconManager.DestroyIcon( _currentIcon );
    }

    private Icon? _currentIcon;
    private readonly SynchronizationContext _uiContext = uiContext;
    private readonly NotifyIcon _notifyIconCtrl = notifyIconCtrl;
    private readonly IIconManager _iconManager = iconManager;
    private readonly AppSettings _settings = settings;
}
