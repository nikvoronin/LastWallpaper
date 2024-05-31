using LastWallpaper.Pods;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LastWallpaper;

internal static class Program
{
    [STAThread]
    static int Main()
    {
        ApplicationConfiguration.Initialize();

        var scheduler = new Scheduler( [
            new BingMay24()
            ] );

        NotifyIcon notifyIconCtrl =
            new() {
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
                null,
                (_,_) => {
                    scheduler.Update();

                    // TODO: replace stub with update process
                    ToastNotifications.ShowToast(
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.MyPictures),
                            "bingImage.jpg"),
                        AppName,
                        "© Nikolai Voronin");
                } )
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

    const string UpdateCtxMenuItemName = nameof( UpdateCtxMenuItemName );
    const string AppName = "The Last Wallpaper";
    const string AppVersion = "4.5.31-alpha";
    const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    internal enum ErrorLevel
    {
        ExitOk = 0
    }
}