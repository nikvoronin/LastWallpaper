using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Handlers;
using LastWallpaper.Logic.Icons;
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

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent", settings.UserAgent );

        var notifyIconCtrl =
            new NotifyIcon() {
                Text = AppName,
                Visible = false,
                Icon = SystemIcons.GetStockIcon(
                    StockIconId.ImageFiles )
            };

        var resourceManager = new ResourceManager();
        var activePods =
#if !DEBUG
            settings.ActivePods.Distinct()
#else
            new PodType[] { PodType.Bing, PodType.Apod, PodType.Wikipedia, PodType.Elementy }
#endif
            .Select( podType =>
                PodsFactory.Create(
                    podType,
                    httpClient, resourceManager,
                    settings ) )
            .OfType<IPotdLoader>()
            .ToList();

        if (activePods.Count == 0) // TODO: add logger
            return (int)ErrorLevel.NoPodsDefined;

        var frontUpdateHandler =
            new FrontUpdateHandler(
                SynchronizationContext.Current!,
                notifyIconCtrl,
                IconManagerFactory.Create( settings.TrayIcon ),
                settings );

        var podsUpdateHandler =
            new PodsUpdateHandler(
                activePods,
                frontUpdateHandler,
                resourceManager );

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                podsUpdateHandler,
                settings );

        var imagoResult = FileManager.LoadLastImago();
        if (imagoResult.IsSuccess) {
            frontUpdateHandler.HandleUpdate(
                new(
                    hasNews: false, 
                    imagoResult.Value ),
                CancellationToken.None );
        }

        notifyIconCtrl.ContextMenuStrip = CreateContextMenu( scheduler );
        notifyIconCtrl.Visible = true;

        scheduler.Start();
        Application.Run();

        scheduler.Dispose();

        notifyIconCtrl.Visible = false;
        notifyIconCtrl.Dispose();

        return (int)ErrorLevel.ExitOk;
    }

    private static ContextMenuStrip CreateContextMenu( Scheduler scheduler )
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
    public const string AppVersion = "4.8.7";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";
}