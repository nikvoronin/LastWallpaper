using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Diagnostics;

namespace LastWallpaper.Logic.Icons;

public static class IconManagerFactory
{
    public static IIconManager Create(TrayIconType iconType)
        => iconType switch
        {
            TrayIconType.Replica => new ReplicaIconManager(),

            TrayIconType.Mosaic => throw new NotImplementedException(),

            _ => throw new UnreachableException(
                $"Unknown tray icon type {iconType}.")
        };
}
