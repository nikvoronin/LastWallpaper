using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace LastWallpaper;

public static class ToastNotifications
{
    public static void ShowToast(
        string filename,
        string? title,
        string? copyright,
        TimeSpan expireIn )
    {
        var toast = new ToastContentBuilder()
            .AddHeroImage( new Uri( filename ) );

        if (title is not null)
            toast.AddText( title ); // should limit length to 250

        if (copyright is not null)
            toast.AddAttributionText( copyright ); // should limit length to 100

        // TODO:? add "about picture" button

        toast.Show( toast => {
            toast.Group = Program.AppName;
            toast.ExpirationTime = DateTime.Now.Add( expireIn );
        } );
    }
}