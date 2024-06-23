using LastWallpaper.Abstractions;
using LastWallpaper.Handlers;
using LastWallpaper.Models;
using LastWallpaper.Pods;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace LastWallpaper;

internal static class Program
{
    [STAThread]
    static int Main()
    {
        AppSettings settings = FileManager.LoadAppSettings();

        ApplicationConfiguration.Initialize();
        SynchronizationContext.SetSynchronizationContext(
            new WindowsFormsSynchronizationContext() );

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add(
            "User-Agent", settings.UserAgent );

        var notifyIconCtrl =
            new NotifyIcon() {
                Text = AppName,
                Visible = false,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles )
            };

        var activePodsOnly =
            settings.ActivePods.Distinct()
            .Select( podType =>
                PodsFactory.CreatePod( podType, client, settings ) )
            .OfType<IPotdLoader>()
            .ToList();

        if (activePodsOnly.Count == 0) // TODO: add logger
            return (int)ErrorLevel.NoPodsDefined;

        var frontUpdateHandler =
            new FrontUpdateHandler(
                SynchronizationContext.Current!,
                notifyIconCtrl,
                settings);

        var podsUpdateHandler =
            new PodsUpdateHandler(
                activePodsOnly, 
                frontUpdateHandler);

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                podsUpdateHandler,
                activePodsOnly,
                settings );

        var imagoResult = FileManager.LoadLastImago();
        if (imagoResult.IsSuccess)
            frontUpdateHandler.RestoreUi( imagoResult.Value );

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
                        try {
                            ExecShellProcess(
                                "explorer", FileManager.AlbumFolder);
                        } catch {}
                    } )
                {
                    Enabled = true,
                    Visible = true
                },

                new ToolStripSeparator(),

                new ToolStripMenuItem(
                    $"&About {AppName} {AppVersion}",
                    null, (_,_) => {
                        try {
                            ExecShellProcess(
                                "cmd", $"/c start {GithubProjectUrl}");
                        } catch {}
                    } ),

                new ToolStripSeparator(),

                new ToolStripMenuItem(
                    "&Quit",
                    null, (_,_) => Application.Exit() )
            ]
        );

        return contextMenu;
    }

    private static void ExecShellProcess(
        string command, string args )
        => Process.Start(
            new ProcessStartInfo( command, args ) {
                CreateNoWindow = true
            } );

    public const string AppName = "The Last Wallpaper";
    public const string AppVersion = "4.6.22";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";
}