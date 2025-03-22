using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Handlers;
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
        var targets = updateParameters.UpdateTargets;
        if (targets == UiUpdateTargets.None) return;

        var updateResult = updateParameters.UpdateResult;

        if (targets.HasFlag( UiUpdateTargets.NotifyIcon )) {
            if (_currentIcon is not null)
                _iconManager.DestroyIcon( _currentIcon );

            try {
                _notifyIconCtrl.Icon = _iconManager.CreateIcon( updateResult.Filename );
                _currentIcon = _notifyIconCtrl.Icon;
            }
            catch (FileNotFoundException) { }

            _notifyIconCtrl.Text =
                $"{Program.AppName} #{updateResult.PodType.ToPodName()}\n{updateResult.Created:D} {updateResult.Created:t}";
        }

        if (targets.HasFlag( UiUpdateTargets.Toast )) {
            _uiContext?.Post( _ =>
                ToastNotifications.ShowToast(
                    updateResult.Filename,
                    updateResult.Title,
                    updateResult.Copyright,
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
