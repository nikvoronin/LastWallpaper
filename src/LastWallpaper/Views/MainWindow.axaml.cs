using Avalonia.Controls;
using PropertyChanged;
using System;

namespace LastWallpaper.Views
{
    [DoNotNotify]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing( object? sender, System.ComponentModel.CancelEventArgs e )
        {
            e.Cancel = true;
            Hide();
        }
    }
}