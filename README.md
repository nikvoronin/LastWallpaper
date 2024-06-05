# The Last Wallpaper

Wallpaper of the day on desktop.

- [What's New](#whats-new)
- [Documentation](#documentation)
- [Release Notes](#release-notes)

![20240603_212225](https://github.com/nikvoronin/LastWallpaper/assets/11328666/921585aa-f489-43e6-b084-7f5db9006428)

__System requirements:__ Windows 10 x64, .NET Desktop Runtime 8.0.

> There are at least two others developer versions in branches: one is cross-platform app written under the AvaloniaUI /[develop-cs-xplatform](https://github.com/nikvoronin/LastWallpaper/tree/develop-cs-xplatform) and another one /[develop-fs-win](https://github.com/nikvoronin/LastWallpaper/tree/develop-fs-win) written in F#. Both are workable but abandoned and obsolete.

## What's New

3 Jun 2024

- Bing POD loader is ready.

Not so far

- Switched back to the csharp + winforms.

See also [Release Notes](#release-notes) chapter.

## Documentation

- Bing image of the day. API:
    - [May 2024](/docs/bing_may-2024.md)
    - [Nov 2022](/docs/bing_nov-2022.md)
- [NASA APIs](https://api.nasa.gov/), free but limited with 50 requests per IP address per day.
- [Windows 10 Toast Notifications](/docs/win10_toast_notifications.md)
- [Windows Desktop Wallpaper](/docs/windows_desktop_wallpaper.md)

## Release Notes

| Version       | Notes                                                                 |
| ------------- | --------------------------------------------------------------------- |
| 4.6.3         | Bing POD loader is ready                                              |
| 4.5.29-alpha  | C#, switched back to csharp + winforms                                |
| 3.6.19-alpha  | Windows toast notifications                                           |
| 3.6.18-alpha  | F#, Bing POD. Changes tray icon according to the new image of the day |
| 2.11.22-alpha | C#, cross-platform version with Avalonia UI                           |
