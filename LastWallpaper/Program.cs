using LastWallpaper.Pods.Bing;
using LastWallpaper.Pods.Nasa;
using LastWallpaper.Pods.Wikimedia;
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
        client.DefaultRequestHeaders.Add( "User-Agent", HttpClientUserAgent );

        var notifyIconCtrl =
            new NotifyIcon() {
                Text = AppName,
                Visible = false,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles )
            };

        var updateUiHandler =
            new UpdateUiHandler(
                    SynchronizationContext.Current!,
                    notifyIconCtrl );

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                updateUiHandler,
                [
                    new BingPodLoader(client),
                    new NasaApodLoader(client),
                    new WikipediaPodLoader(client)
                ] );

        var imagoResult = FileManager.LoadLastImago();
        if (imagoResult.IsSuccess)
            updateUiHandler.InitialUpdate( imagoResult.Value );

        notifyIconCtrl.ContextMenuStrip =
            CreateContextMenu( scheduler );
        notifyIconCtrl.Visible = true;

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
        contextMenu.Items.AddRange(
            [
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
                    null, (_,_) => Application.Exit() )
            ]
        );

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

    private const string HttpClientUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

    public const string AppName = "The Last Wallpaper";
    public const string AppVersion = "4.6.16";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    internal enum ErrorLevel
    {
        ExitOk = 0
    }
}