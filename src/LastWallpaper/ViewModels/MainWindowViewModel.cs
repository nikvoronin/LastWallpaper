using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;

namespace LastWallpaper.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public string? ImageTitle { get; set; } = "The Last Wallpaper";
        public Bitmap? ImageSource { get; set; }
    }
}