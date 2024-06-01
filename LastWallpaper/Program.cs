using LastWallpaper.Pods;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace LastWallpaper;

internal static class Program
{
    [STAThread]
    static int Main()
    {
        ApplicationConfiguration.Initialize();
        SynchronizationContext.SetSynchronizationContext(
            new WindowsFormsSynchronizationContext() );

        var client = new HttpClient();

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                new UpdateUiHandler( SynchronizationContext.Current! ),
                [
                    new BingLoader(client)
                ] );

        var notifyIconCtrl =
            new NotifyIcon() {
                Text = AppName,
                Visible = true,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles ), // TODO: replace with icon
                ContextMenuStrip = CreateContextMenu( scheduler )
            };

        scheduler.Start();

        Application.Run();

        scheduler.Dispose();

        notifyIconCtrl.Visible = false;
        notifyIconCtrl.Dispose();

        return (int)ErrorLevel.ExitOk;
    }

    private static ContextMenuStrip CreateContextMenu(
        Scheduler scheduler )
    {
        ContextMenuStrip contextMenu = new();
        contextMenu.Items.AddRange( new ToolStripItem[] {
            new ToolStripMenuItem(
                "&Update Now!",
                null, (_,_) => scheduler.Update() )
            {
                Name = UpdateCtxMenuItemName,
                Enabled = true,
                Visible = true
            },

            new ToolStripSeparator(),

            new ToolStripMenuItem(
                $"&About {AppName} {AppVersion}",
                null,
                (_,_) => {
                    try {
                        Process.Start(
                            new ProcessStartInfo(
                                "cmd",
                                $"/c start {GithubProjectUrl}")
                            {
                                CreateNoWindow = true
                            });
                    } catch {}
                } ),

            new ToolStripSeparator(),

            new ToolStripMenuItem(
                "&Quit",
                null, (_,_) => Application.Exit() ),
        } );

        return contextMenu;
    }

    public const string AppName = "The Last Wallpaper";
    public const string AppVersion = "4.6.1-alpha";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    private const string UpdateCtxMenuItemName = nameof( UpdateCtxMenuItemName );

    internal enum ErrorLevel
    {
        ExitOk = 0
    }
}