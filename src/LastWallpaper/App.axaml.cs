using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using LastWallpaper.Models;
using LastWallpaper.ViewModels;
using LastWallpaper.Views;
using Microsoft.Toolkit.Uwp.Notifications;
using PropertyChanged;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace LastWallpaper
{
    [DoNotNotify]
    public partial class App : Application
    {
        public const string DefaultWallpapersFolder = "Wallpapers";
        public const string ApplicationGithubRepositoryLink
            = "https://github.com/nikvoronin/LastWallpaper";

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

        public App()
        {
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
                () => OpenExternalBrowser( Path.GetFullPath( DefaultWallpapersFolder ) ) );
        }

        public override void Initialize()
        {
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
            _bingLoader = new BingLoader( screenSize, DefaultWallpapersFolder );
            _bingLoader.ImageUpdated += OnBingImageUpdated;

            desktop.Startup += Desktop_Startup;
            desktop.Exit += Desktop_Exit;

            base.OnFrameworkInitializationCompleted();
        }

        private void OnBingImageUpdated( object? sender, ImageOfTheDayInfo info )
        {
            var desktop = Desktop;

            Dispatcher.UIThread.InvokeAsync( () => {
                if ( desktop?.MainWindow.DataContext is not MainWindowViewModel vm )
                    return;

                vm.ImageSource = new Bitmap( info.FileName );
                vm.ImageTitle = $"{info.Description}\n{info.Copyright}";
            } );

            // TODO: add ability to disable toasts. Enable by default
            SendToastNotification( info );

            try {
                WindowsRegistry.SetWallpaper( info.FileName );
            }
            catch { }
        }

        private static void SendToastNotification( ImageOfTheDayInfo info )
        {
            // https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/send-local-toast?tabs=desktop
            // https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/adaptive-interactive-toasts?tabs=appsdk

            new ToastContentBuilder()
                .AddHeroImage( new Uri( info.FileName ) )
                .AddText( info.Description )
                .AddAttributionText( info.Copyright )
                .Show( toast => {
                    toast.Group = "The Last Wallpaper";
                    toast.ExpirationTime = DateTime.Now.AddDays( 2 ); // TODO: add expiration as option
                } );
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

        private static void OpenExternalBrowser( string url )
        {
            if ( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) ) {
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