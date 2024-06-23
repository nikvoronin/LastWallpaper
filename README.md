# The Last Wallpaper

Picture Of The Day (POTD) as windows desktop wallpaper.

- [What's New](#whats-new)
- [Documentation](#documentation)
- [Release Notes](#release-notes)

![20240603_212225](https://github.com/nikvoronin/LastWallpaper/assets/11328666/921585aa-f489-43e6-b084-7f5db9006428)

__System requirements:__ Windows 10 x64, .NET Desktop Runtime 8.0.

> There are at least two others developer versions in branches: one is cross-platform app written under the AvaloniaUI /[develop-cs-xplatform](https://github.com/nikvoronin/LastWallpaper/tree/develop-cs-xplatform) and another one /[develop-fs-win](https://github.com/nikvoronin/LastWallpaper/tree/develop-fs-win) written in F#. Both are workable but abandoned and obsolete.

## What's New

23 Jun 2024

- Add application settings file.
- Remember the last wallpaper and recall it after the app restarts.

Not so far

- Wikipedia POTD loader added.
- NASA APOD loader added.
- Bing POD loader is ready.
- Switched back to the csharp + winforms.

See also [Release Notes](#release-notes) chapter.

## Documentation

- // TODO: https://www.natgeotv.com/ca/photo-of-the-day
- [Wikipedia POTD](/docs/wikipedia_jun-2024.md) (Picture Of The Day)
- Bing image of the day. API:
    - [May 2024](/docs/bing_may-2024.md)
    - [Nov 2022](/docs/bing_nov-2022.md)
- [NASA Open APIs Portal](https://api.nasa.gov/), free but limited with 50 requests per IP address per day.
- [Windows 10 Toast Notifications](/docs/win10_toast_notifications.md)
- [Windows Desktop Wallpaper](/docs/windows_desktop_wallpaper.md)

## Application Settings

See [appsettings.json](https://github.com/nikvoronin/LastWallpaper/blob/main/LastWallpaper/appsettings.json) file in the application folder.

- __update_every__ - check pod updates every.
- __toast_expire_in__ - toast message will disappear after this period of time.
- __active_pods__ - active pods list. Will updated in appear order. The first one with positive result become a wallpaper.
- __bing__
    - __resolution__ - resolution of the picture:
        - "UHD" - 4K
        - "1920x1080" - FullHD
        - "1280x720" - HD
- __apod__
    - __throttling_hours__ - next update after this period. Be aware that APOD free but limited with 50 requests per IP address per day.
    - __api_key__ - if you have you payed api key.

```json
{
  "update_every": "00:57:00",
  "toast_expire_in": "2.00:00:00", // days . hours : minutes : seconds

  "active_pods": [ "bing", "wikipedia", "apod" ],
  
  "bing": {
    "resolution": "UHD"
  },

  "apod": {
    "throttling_hours": "23:00:00",
    "api_key": "DEMO_KEY"
  },

  "wikipedia": {
  },

  "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"
}
```

## Release Notes

| Version       | Notes                                                                 |
| ------------- | --------------------------------------------------------------------- |
| 4.6.23        | Add application settings file.<br/>Remember the last wallpaper and recall it after the app restarts.                                        |
| 4.6.10        | Wikipedia POTD loader added                                           |
| 4.6.5         | NASA APOD loader added                                                |
| 4.6.3         | Bing POD loader is ready                                              |
| 4.5.29-alpha  | C#, switched back to csharp + winforms                                |
| 3.6.19-alpha  | Windows toast notifications                                           |
| 3.6.18-alpha  | F#, Bing POD. Changes tray icon according to the new image of the day |
| 2.11.22-alpha | C#, cross-platform version with Avalonia UI                           |
