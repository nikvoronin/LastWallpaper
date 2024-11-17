# The Last Wallpaper

Picture Of The Day (POTD) on desktop.\
-- Bing, NASA, Wikipedia, Elementy, AstroBin, NatGeoTV, Copernicus

![20240603_212225](https://github.com/nikvoronin/LastWallpaper/assets/11328666/921585aa-f489-43e6-b084-7f5db9006428)

- [What's New](#whats-new)
- [Documentation](#documentation)
  - [Media sources +8](#media-sources-8)
  - [TODO? Сustom POD loader](#todo-сustom-pod-loader)
- [Application Settings](#application-settings)
- [Release Notes](#release-notes)

__System requirements:__ Windows 10 x64, .NET Desktop Runtime 8.0.

> There are at least two others developer versions in branches: one is cross-platform app written under the AvaloniaUI /[develop-cs-xplatform](https://github.com/nikvoronin/LastWallpaper/tree/develop-cs-xplatform) and another one /[develop-fs-win](https://github.com/nikvoronin/LastWallpaper/tree/develop-fs-win) written in F#. Both are workable but abandoned and obsolete.

## What's New

13 Nov 2024

- Improve validating of wikipedia media types.
- Rebuild mosaic in tray icon on user click (see [tray_icon:mosaic](#application-settings) option).

Not so far

- Control manual change of system wallpaper.
- Add NASA.gov POD loader, general image-of-the-day page.
- Add Copernicus POD, a gallery containing the newest Copernicus Sentinel images.
- Add NatGeoTV POD.

See also [Release Notes](#release-notes) chapter.

## Documentation

### Media sources +8

- [Copernicus](/docs/copernicus_aug-2024.md) - The daily updated gallery containing the newest Copernicus Sentinel images.
- [NatGeoTV Canada](/docs/natgeotv_aug-2024.md) - Photo of the day - National Geographic Channel - Canada.
- [AstroBin](/docs/astrobin_aug-2024.md) - Home of astrophotography.
- [Elementy RSS Feed](/docs/elementy_aug-2024.md) - Science picture of the day.
- [Wikipedia](/docs/wikipedia_jun-2024.md) - Picture Of The Day.
- Bing image of the day API
    - [May 2024](/docs/bing_may-2024.md)
    - [Nov 2022](/docs/bing_nov-2022.md)
- NASA
  - [Image of the Day](/docs/nasagov_aug-2024.md)
  - [Open APIs Portal](https://api.nasa.gov/) - free but limited with 50 requests per IP address per day.
- [Windows 10 Toast Notifications](/docs/win10_toast_notifications.md)
- [Windows Desktop Wallpaper](/docs/windows_desktop_wallpaper.md)
- TODO?
  - flickr.com/groups/imageoftheday/pool/ - a group with daily image posting.
  - https://epod.usra.edu/blog/
  - https://www.the-scientist.com/type/image-of-the-day
  - https://bpod.org.uk/

### TODO? Сustom POD loader

- PodsFactory
- PodLoader
- Scheduler
- HttpPodLoader
- HtmlPodLoader
- ResultsProcessor
- ToastNotifications
- WindowsRegistry
- RssReader

## Application Settings

See [appsettings.json](https://github.com/nikvoronin/LastWallpaper/blob/main/LastWallpaper/appsettings.json) file in the application folder.

Format for date-time options (periods/timeout/etc) is: `days.hours:minutes:seconds.milliseconds`.\
For ex.: 5 days 4 hour 3 minutes 2 seconds and 789 milliseconds = "5.04:03:02.789".

- __update_every__ - check pod updates every.
- __update_timeout__ - timeout for updating all pods.
- __toast_expire_in__ - toast message will disappear after this period of time.
- __tray_icon__ - what will the tray icon look like:
  - __replica__ - a tiny replica of desktop image.
  - __mosaic__ - nine tiles with main accent colors of desktop image.
- __wallpaper_fit__ - specifies how the desktop wallpaper should be displayed.
  - __default__ - the best fit. It is equal to "fill".
  - __center__ - center the image; do not stretch.
  - __tile__ - tile the image across all monitors.
  - __stretch__ - stretch the image to exactly fit on the monitor, without maintain aspect ratio.
  - __span__ - spans a single image across all monitors attached to the system.
  - __fit__ - stretch the image to exactly the height or width of the monitor without changing its aspect ratio or cropping the image. This can result in colored letterbox bars on either side or on above and below of the image.
  - __fill__ - stretch the image to fill the screen, cropping the image as necessary to avoid letterbox bars. This one is used as "default".
- __active_pods__ - active pods list. Will updated in appear order. The first one with positive result become a wallpaper.
  - __bing__ - bing.com
  - __wikipedia__ - Wikipedia POTD.
  - __apod__ - NASA APOD.
  - __elementy__ - Elementy (science picture of the day).
  - __astrobin__ - AstroBin IOTD.
  - __natgeotv__ - NatGeoTV POD.
  - __copernicus__ - Copernicus IOTD.
  - __nasa__ - NASA.gov POTD.
- __bing__
    - __resolution__ - resolution of the picture:
        - __UltraHD__ or __UHD__ - 4K, 3840x2160 px
        - __FullHD__ or __FHD__ - 1920x1080 px
        - __HD__ - 1280x720 px
- __apod__
    - __throttling_hours__ - next update will happen after this period. Be aware that APOD free but limited with 50 requests per IP address per day.
    - __api_key__ - if you have your own paid API key.

```json
{
  "update_every": "00:57:00",
  "update_timeout": "00:05:00",
  "toast_expire_in": "2.00:00:00",
  "tray_icon": "replica",
  "wallpaper_fit": "fill",

  "active_pods": [ 
    "bing", 
    "apod",
    "elementy",
    "wikipedia"
  ],
  
  "bing": {
    "resolution": "UHD"
  },

  "apod": {
    "throttling_hours": "23:00:00",
    "api_key": "DEMO_KEY"
  },

  "user_agent": "LastWallpaper/4.6.23 (Windows NT 10.0; Win64; x64)"
}
```

## Release Notes

### 4.11.13 --fix

- Fix tray icon mouse click.

### 4.11.12

- Improve validating of wikipedia media types.
- Rebuild mosaic in tray icon on user click (see [tray_icon:mosaic](#application-settings) option).

### 4.9.30 --fix

- Fix using of universal datetime, local one used instead.

### 4.9.21

- Control manual change of system wallpaper.
- Add NASA.gov POD loader, general image-of-the-day page.
- Add Copernicus POD, a gallery containing the newest Copernicus Sentinel images.
- Add NatGeoTV POD.

### 4.8.18

- Add mosaic tray icon.
- Add AstroBin - home of astrophotography.
- Add configurable wallpaper fit parameter.
- Add Elementy (science picture of the day).

### 4.6.24

- Add application settings file.
- Remember the last wallpaper and recall it after the app restarts.

### 4.6.10

- Wikipedia POTD loader added

### 4.6.5

- NASA APOD loader added

### 4.6.3

- Bing POD loader is ready

### 4.5.29-alpha

- C#, switched back to csharp + winforms

### 3.6.19-alpha

- Windows toast notifications

### 3.6.18-alpha

- F#, Bing POD. Changes tray icon according to the new image of the day

### 2.11.22-alpha

- C#, cross-platform version with Avalonia UI.
