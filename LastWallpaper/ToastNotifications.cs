﻿using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace LastWallpaper;

public static class ToastNotifications
{
    public const string ToastGroupName = "The Last Wallpaper";

    public static void ShowToast(
        string filename,
        string? title,
        string? copyright )
    {
        var toast = new ToastContentBuilder()
            .AddHeroImage( new Uri( filename ) );

        if (title is not null)
            toast.AddText( title );

        if (copyright is not null)
            toast.AddAttributionText( copyright );

        toast.Show( toast => {
            toast.Group = ToastGroupName;
            toast.ExpirationTime = DateTime.Now.AddDays( 2 ); // TODO: add expiration as option
        } );
    }
}