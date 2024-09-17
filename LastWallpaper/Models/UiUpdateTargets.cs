using System;

namespace LastWallpaper.Models;

[Flags]
public enum UiUpdateTargets
{
    None = 0,

    NotifyIcon = 1,
    Toast = 2,

    All = NotifyIcon | Toast
}
