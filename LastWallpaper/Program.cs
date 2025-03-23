using LastWallpaper.Abstractions;
using LastWallpaper.Logic;
using LastWallpaper.Logic.Handlers;
using LastWallpaper.Logic.Icons;
using LastWallpaper.Models;
using LastWallpaper.Pods;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Forms;

namespace LastWallpaper;

internal static class Program
{
    [STAThread]
    static int Main()
    {
        #region Application Folders
        var appFolder =
            Path.GetDirectoryName( Application.ExecutablePath )!;

        var cacheFolder =
            Path.Combine( appFolder, CacheFolderName );

        if (!Directory.Exists( cacheFolder ))
            Directory.CreateDirectory( cacheFolder );

        var resourceManager =
            new ResourceManager(
                appFolder,
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyPictures ),
                    AppName ),
                cacheFolder );
        #endregion

        AppSettings settings =
            LoadAppSettings( Path.Combine( appFolder, AppSettingsFileName ) );

        // TODO: add application and domain exception handlers

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
                Icon =
                    SystemIcons.GetStockIcon(
                        StockIconId.ImageFiles )
            };

        var activePods =
#if DEBUG
            Enum.GetValues<PodType>()
#else
            settings.ActivePods.Distinct()
#endif
            .Select( podType =>
                PodsFactory.Create(
                    podType,
                    httpClient,
                    resourceManager,
                    new RssReader(),
                    settings ) )
            .OfType<IPotdLoader>()
            .ToList();

        if (activePods.Count == 0) // TODO: add logger
            return (int)ErrorLevel.NoPodsDefined;

        var frontUpdateHandler =
            new FrontUpdateHandler(
                SynchronizationContext.Current!,
                notifyIconCtrl,
                IconManagerFactory.Create( settings.TrayIconStyle ),
                settings );

        var podsUpdateHandler =
            new PodsUpdateHandler(
                activePods,
                frontUpdateHandler,
                new ResultsProcessor(
                    resourceManager,
                    settings ) );

        Debug.Assert( SynchronizationContext.Current is not null );
        var scheduler =
            new Scheduler(
                podsUpdateHandler,
                settings );

        UpdateTrayIconOnly();

        notifyIconCtrl.ContextMenuStrip =
            CreateContextMenu(
                scheduler,
                resourceManager );
        notifyIconCtrl.MouseClick += OnMouseLeftButtonClick;
        notifyIconCtrl.Visible = true;

        scheduler.Start();
        Application.Run();

        scheduler.Dispose();

        notifyIconCtrl.Visible = false;
        notifyIconCtrl.MouseClick -= OnMouseLeftButtonClick;
        notifyIconCtrl.Dispose();

        return (int)ErrorLevel.ExitOk;

        void OnMouseLeftButtonClick( object? _, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left
                && settings.TrayIconStyle == TrayIconType.Mosaic)
                UpdateTrayIconOnly();
        }

        void UpdateTrayIconOnly()
        {
            var imagoResult = resourceManager.RestoreLastWallpaper();
            if (imagoResult.IsSuccess) {
                frontUpdateHandler.HandleUpdate(
                    new FrontUpdateParameters(
                        UiUpdateTargets.NotifyIcon,
                        imagoResult.Value ),
                    CancellationToken.None );
            }
        }
    }

    private static ContextMenuStrip CreateContextMenu(
        Scheduler scheduler,
        ResourceManager resourceManager )
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
                                "explorer", resourceManager.AlbumFolder);
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

    private static AppSettings LoadAppSettings( string appSettingsFileName )
    {
        AppSettings? appSettings = null;

        if (File.Exists( appSettingsFileName )) {
            try {
                appSettings =
                    JsonSerializer.Deserialize<AppSettings>(
                        File.ReadAllText( appSettingsFileName ),
                        _jsonSerializerOptions );
            }
            catch { }
        }

        return appSettings ?? new();
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
            }
        };

    public const string AppName = "The Last Wallpaper";
    public const string AppVersion = "5.3.23-dev";
    public const string GithubProjectUrl = "https://github.com/nikvoronin/LastWallpaper";

    private const string CacheFolderName = "cache";
    public const string LastWallpaperFileName = "lastwallpaper.json";
    private const string AppSettingsFileName = "appsettings.json";
}