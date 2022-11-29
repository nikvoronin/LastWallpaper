using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using LastWallpaper.Core;
using LastWallpaper.Core.Model;
using LastWallpaper.ViewModels;
using LastWallpaper.Views;
using PropertyChanged;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace LastWallpaper
{
    [DoNotNotify]
    public partial class App : Application
    {
        public const string ApplicationGithubRepositoryLink
            = "https://github.com/nikvoronin/LastWallpaper";
        public const string CurrentFolderRelativePathPrefix = "./";
        public const string DefaultWallpapersFolderName = "The Last Wallpaper";

        public readonly string WallpapersFolder;

        BingLoader? _bingLoader;

        public ICommand ExitCommand { get; private set; }
        public ICommand UpdateNowCommand { get; private set; }
        public ICommand ShowMainWindowCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand OpenWallpapersFolderCommand { get; private set; }

        public IClassicDesktopStyleApplicationLifetime? Desktop
            => ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        public RelayCommand RelayCommandFor( Action<IClassicDesktopStyleApplicationLifetime> act )
            => new( () => {
                var desktop = Desktop;
                if ( desktop is not null )
                    act( desktop );
            } );

        private static string? MyPicturesFolderOrNull()
        {
            try {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.MyPictures );
            }
            catch { return null; }
        }

        public App()
        {
            WallpapersFolder =
                Path.Combine(
                    MyPicturesFolderOrNull() ?? CurrentFolderRelativePathPrefix,
                    DefaultWallpapersFolderName );

            ExitCommand = RelayCommandFor( desk => desk.Shutdown() );

            ShowMainWindowCommand = RelayCommandFor( desk => {
                var normalizeState =
                    desk.MainWindow.WindowState != WindowState.Normal
                    && desk.MainWindow.WindowState != WindowState.Maximized;

                if ( normalizeState )
                    desk.MainWindow.WindowState = WindowState.Normal;

                desk.MainWindow.Show();
            } );

            UpdateNowCommand = new RelayCommand(
                () => _bingLoader?.Update() );

            AboutCommand = new RelayCommand(
                () => OpenExternalBrowser( ApplicationGithubRepositoryLink ) );

            OpenWallpapersFolderCommand = new RelayCommand(
                () => {
                    var path = Path.GetFullPath( WallpapersFolder );
                    if ( IsWindowsPlatform ) path = $"explorer \"{path}\"";
                    OpenExternalBrowser( path );
                } );
        }

        private const object StaticClassInstance = null; // yes, it is null. static classes has no instance

        MethodInfo? _showToastMethod = null; // STUB: replace with something also
        MethodInfo? _setWindowsRegistryWallpaperMethod = null; // STUB: replace with something also
        public override void Initialize()
        {
            // TODO: discover and load plugins here

            if ( IsWindowsPlatform ) {
                // STUB: for windows toasts plugin
                try {
                    var toastPluginPath = Path.GetFullPath(
                        "./Extensions/ToastNotifications/ToastNotifications.dll" );
                    Assembly toastPlugin =
                        PluginLoadContext.LoadPluginFromFile( toastPluginPath );

                    Type? notificationManagerClass =
                        toastPlugin.GetType( "LastWallpaper.ToastNotifications" );

                    _showToastMethod =
                        notificationManagerClass?.GetMethod( "OnImageUpdated" );
                }
                catch { }

                // STUB: for windows registry plugin
                try {
                    var windowsRegistryPluginPath = Path.GetFullPath(
                        "./Extensions/WindowsRegistry/WindowsRegistry.dll" );
                    Assembly windowsRegistry =
                        PluginLoadContext.LoadPluginFromFile( windowsRegistryPluginPath );

                    Type? notificationManagerClass =
                        windowsRegistry.GetType( "LastWallpaper.WindowsRegistry" );

                    _setWindowsRegistryWallpaperMethod =
                        notificationManagerClass?.GetMethod( "OnImageUpdated" );
                }
                catch { }
            }

            // here and above is a last chance to initialize relay commands
            AvaloniaXamlLoader.Load( this );
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var desktop = Desktop;
            if ( desktop is null ) return;

            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            ExpressionObserver.DataValidators.RemoveAll( x => x is DataAnnotationsValidationPlugin );
            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel(),
                WindowState = WindowState.Minimized
            };

            PixelSize screenSize =
                desktop.MainWindow.Screens.Primary
                .Bounds.Size;

            // TODO: try get a wallpaper folder from app-settings
            _bingLoader = new BingLoader( screenSize, WallpapersFolder );
            _bingLoader.ImageUpdated += OnBingImageUpdated;

            desktop.Startup += Desktop_Startup;
            desktop.Exit += Desktop_Exit;

            base.OnFrameworkInitializationCompleted();
        }

        private void OnBingImageUpdated( object? sender, ImageOfTheDay info )
        {
            var desktop = Desktop;

            Dispatcher.UIThread.InvokeAsync( () => {
                if ( desktop?.MainWindow.DataContext is not MainWindowViewModel vm )
                    return;

                vm.ImageSource = new Bitmap( info.FileName );
                vm.ImageTitle = $"{info.Description}\n{info.Copyright}";
            } );

            if ( IsWindowsPlatform ) {
                // STUB: remove hardcoded plugins call
                // TODO: add ability to disable toasts. Enable by default
                _showToastMethod
                    ?.Invoke( StaticClassInstance, new object[] { info } );
                _setWindowsRegistryWallpaperMethod
                    ?.Invoke( StaticClassInstance, new object[] { info } );
            }
        }

        private void Desktop_Startup( object? sender, ControlledApplicationLifetimeStartupEventArgs e )
        {
            Desktop.MainWindow.Opened += MainWindow_Opened;

            _bingLoader?.StartPoll();
        }

        // HACK: to hide main window at the first start
        private void MainWindow_Opened( object? sender, EventArgs e )
        {
            var desk = Desktop;
            desk.MainWindow.Opened -= MainWindow_Opened;
            desk.MainWindow.Hide();
        }

        private void Desktop_Exit( object? sender, ControlledApplicationLifetimeExitEventArgs e )
        {
            _bingLoader?.StopPoll();
        }

        private static readonly bool IsWindowsPlatform =
            RuntimeInformation.IsOSPlatform( OSPlatform.Windows );

        private static void OpenExternalBrowser( string url )
        {
            if ( IsWindowsPlatform ) {
                Process.Start(
                    new ProcessStartInfo( "cmd", $"/c start {url.Replace( "&", "^&" )}" ) );
            }
            else if ( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
                Process.Start( "xdg-open", url );
            else if ( RuntimeInformation.IsOSPlatform( OSPlatform.OSX ) )
                Process.Start( "open", url );
        }
    }
}