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

        NotifyIcon notifyIconCtrl =
            new() {
                Text = AppName,
                Visible = true,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles), // TODO: replace with icon
                ContextMenuStrip = CreateContextMenu()
            };

        Application.Run();

        notifyIconCtrl.Visible = false;
        notifyIconCtrl.Dispose();

        return (int)ErrorLevel.ExitOk;
    }

    private static ContextMenuStrip CreateContextMenu()
    {
        ContextMenuStrip contextMenu = new();
        contextMenu.Items.AddRange( new ToolStripItem[] {
            new ToolStripMenuItem(
                "&Update Now!",
                null,
                (_,_) => { // TODO: replace stub with update process
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
    const string AppVersion = "4.5.29-alpha";
    const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    internal enum ErrorLevel
    {
        ExitOk = 0
    }
}