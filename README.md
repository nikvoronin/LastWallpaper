# The Last Wallpaper

Wallpaper of the day on desktop.

- [What's New](#whats-new)
- [Documentation](#documentation)
- [Release Notes](#release-notes)

> There are at least two others developer versions in branches: one is cross-platform under the AvaloniaUI /[develop-cs-xplatform](https://github.com/nikvoronin/LastWallpaper/tree/develop-cs-xplatform) and another one /[develop-fs-win](https://github.com/nikvoronin/LastWallpaper/tree/develop-fs-win) written in F#. Both are workable but abandoned and obsolete.

## What's New

29 May 2024

- Switched back to the csharp + winforms.

Not so far

- Initial downloading for NASA APOD image of the day.
- Windows toast notifications.
- Looking for changes from bing (once an hour).
- Downloads new image.
- Updates desktop wallpaper.
- Changes tray icon according to the new image of the day.

See also [Release Notes](#release-notes) chapter.

## Documentation

- [Windows 10 Toast Notifications](/docs/win10_toast_notifications.md).
- [Windows Desktop Wallpaper](/docs/windows_desktop_wallpaper.md).
- [Bing image of the day](/docs/bing_nov-2022.md). --Nov 2022

## Release Notes

<!-- omit in toc -->
### 4.5.29-alpha

- Stub for csharp version. Switched back to the csharp + winforms.

<!-- omit in toc -->
### 3.6.19-alpha

- Windows toast notifications.

<!-- omit in toc -->
### 3.6.18-alpha

- Looking for changes from bing (once an hour).
- Downloads new image.
- Updates desktop wallpaper.
- Changes tray icon according to the new image of the day.

<!-- omit in toc -->
### 0.1.0-alpha

- Bing image of the day. Windows 10 and above only.
- Windows' toast messages.
- --- *C#, 22 Nov 2022*

<!-- omit in toc -->
## Stage

```text
TODO
- icon with nine image dominant colors
- add app global state record
- store tray icon in the global state and dispose it when a new one arrived
> unify providers
    > NASA APOD
        - Use AsyncLoad
    - Bing
- on release, rename .exe file to `The Last Wallpaper`

DONE
--- Options instead of Toast::setupOpt
+ toast notifications
+ update desktop wallpaper
+ shrink image to icon using nearest neighbor method
```
