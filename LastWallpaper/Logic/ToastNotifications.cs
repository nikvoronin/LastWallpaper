using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace LastWallpaper.Logic;

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

        if (title is not null) {
            toast.AddText(
                title.Truncate( TitleMaxLength ) );
        }

        if (copyright is not null) {
            toast.AddAttributionText(
                copyright.Truncate( CopyrightMaxLength ) );
        }

        // TODO? add "about picture" button

        toast.Show( toast => {
            toast.Group = Program.AppName;
            toast.ExpirationTime = DateTime.Now.Add( expireIn );
        } );
    }

    private const int TitleMaxLength = 250;
    private const int CopyrightMaxLength = 100;
}