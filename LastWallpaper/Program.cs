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

        var notifyIconCtrl =
            new NotifyIcon() {
                Text = AppName, // TODO: add last updating date-time from config
                Visible = true,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles ) // TODO: replace with latest icon from config
            };

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                new UpdateUiHandler(
                    SynchronizationContext.Current!,
                    notifyIconCtrl),
                [
                    new BingPodLoader(client)
                ] );

        notifyIconCtrl.ContextMenuStrip =
            CreateContextMenu( scheduler );

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
                Enabled = true,
                Visible = true
            },

            new ToolStripMenuItem(
                "&Open Picture Gallery",
                null, (_,_) => {
                    ExecShellProcess(
                        "explorer", FileManager.AlbumFolder);
                } )
            {
                Enabled = true,
                Visible = true
            },

            new ToolStripSeparator(),

            new ToolStripMenuItem(
                $"&About {AppName} {AppVersion}",
                null, (_,_) => {
                    ExecShellProcess(
                        "cmd", $"/c start {GithubProjectUrl}");
                } ),

            new ToolStripSeparator(),

            new ToolStripMenuItem(
                "&Quit",
                null, (_,_) => Application.Exit() ),
        } );

        return contextMenu;
    }

    private static void ExecShellProcess( string command, string args )
    {
        try {
            Process.Start(
                new ProcessStartInfo( command, args ) {
                    CreateNoWindow = true
                } );
        }
        catch { }
    }

    public const string AppName = "The Last Wallpaper";
    public const string AppVersion = "4.6.3";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    internal enum ErrorLevel
    {
        ExitOk = 0
    }
}