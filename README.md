# The Last Wallpaper

Picture Of The Day (POTD) on desktop.\
⭐ Bing, NASA, Wikipedia, Elementy, NatGeoTV, Copernicus

![20240603_212225](https://github.com/nikvoronin/LastWallpaper/assets/11328666/921585aa-f489-43e6-b084-7f5db9006428)

- [What's New](#whats-new)
- [User Interface](#user-interface)
- [Documentation](#documentation)
  - [Media sources APIs +7](#media-sources-apis-7)
  - [Development specific](#development-specific)
  - [Custom POD Loader Component Guide](#custom-pod-loader-component-guide)
- [Application Settings](#application-settings)
- [Release Notes](#release-notes)

__System requirements:__ Windows 10 x64, .NET Desktop Runtime 8.0.

> There are at least two others developer versions in branches: one is cross-platform app written under the AvaloniaUI /[develop-cs-xplatform](https://github.com/nikvoronin/LastWallpaper/tree/develop-cs-xplatform) and another one /[develop-fs-win](https://github.com/nikvoronin/LastWallpaper/tree/develop-fs-win) written in F#. Both are workable but abandoned and obsolete.

## What's New

17 Oct 2025

- A new [release v5.10.17](https://github.com/nikvoronin/LastWallpaper/releases/tag/v5.10.17) is now available for download in the [Releases section](https://github.com/nikvoronin/LastWallpaper/releases).
- Added `Launch at Startup` menu item.
- The APOD-web pod now supports fetching pictures directly from web pages.
- [Astronomy Picture of the Day](https://apod.nasa.gov/apod/): due to the lapse in federal government funding, NASA is not updating this website.

Not so far

- [Release Candidate v5.8.5-rc1](https://github.com/nikvoronin/LastWallpaper/releases/tag/v5.8.5-rc1) is now available for download in the [Releases section](https://github.com/nikvoronin/LastWallpaper/releases).
- Fixed saving to the directory with the old year in the new year.
- AstroBin (Home of astrophotography) pod was discontinued due to the API being no longer available.
- Complex refactoring of the inner code base.

See also [Release Notes](#release-notes) chapter.

## User Interface

🐫 There are two types of tray icons (actually, three):

- __Replica__ - a tiny replica of desktop wallpaper image.
- __Mosaic__ - nine tiles with main accent colors of desktop image (tiles shuffled from time to time).

When the application can't update or retrieve the wallpaper image, it shows you the third one - a default system icon.

🐭 Hover your mouse over the tray icon. The appeared hint consist of several parts:

```plain
[The Last Wallpaper] [⭐podname]
[3 June 2024] [16:43]
```

- `The Last Wallpaper` - reminds you that you don't need other wallpaper apps.
- `⭐podname` - the name of the source of the picture-of-the-day.
- `3 June 2024` - the date of the picture-of-the-day (not only today).
- `16:43` - when the POD- image was successfully downloaded.

🎯 Do right mouse click over the tray icon:

- __Update Now!__ - check and update wallpaper immediatelly.
- __Open Picture Gallery__ - open the Explorer with application image folder inside your system Picture Gallery.
- __Launch at Startup__ - automatically start the application when Windows starts and the user logs on.
- __About The Last Wallpaper__ /current.version.number/ - open this web-page in default web browser.
- __Quit__ - close and unload the application.

## Documentation

### Media Sources APIs +7

- [Copernicus](/docs/copernicus_aug-2024.md) - The daily updated gallery containing the newest Copernicus Sentinel images.
- [NatGeoTV Canada](/docs/natgeotv_aug-2024.md) - Photo of the day - National Geographic Channel - Canada.
- [Elementy RSS Feed](/docs/elementy_aug-2024.md) - Science picture of the day.
- [Wikipedia](/docs/wikipedia_jun-2024.md) - Picture Of The Day.
- Bing image of the day API
    - [May 2024](/docs/bing_may-2024.md)
    - [Nov 2022](/docs/bing_nov-2022.md)
- NASA
  - [Image of the Day](/docs/nasagov_aug-2024.md)
  - [APOD](/docs/apodweb_aug-2025.md) - Astronomy Picture of the Day.
  - [Open APIs Portal ![External link](https://en.wikipedia.org/w/skins/Vector/resources/skins.vector.styles/images/link-external-small-ltr-progressive.svg)](https://api.nasa.gov/) - free but limited with 50 requests per IP address per day. Restricted for some countries (403 Forbidden).
- ~~[AstroBin](/docs/astrobin_aug-2024.md)~~ The pod was discontinued due to the API being no longer available.

#### ¿TODO?

- flickr.com/groups/imageoftheday/pool/ - a group with daily image posting.
- https://epod.usra.edu/blog/
- https://www.the-scientist.com/type/image-of-the-day
- https://bpod.org.uk/

### Development Specific

- [Windows 10 Toast Notifications](/docs/win10_toast_notifications.md)
- [Windows Desktop Wallpaper](/docs/windows_desktop_wallpaper.md)

### Custom POD Loader Component Guide

#### Core Components

- PodsFactory - Handles registration and instantiation of new POD instances.
- PodLoader - Base class for POD processing functionality.
- Scheduler - Manages periodic updates of the Picture of the Day (POTD).

#### Fetch Modules

- NewsFetcher - Verifies POD readiness for new content retrieval.
- PotdFetcher - Handles POTD download operations.
- DescriptionFetcher - Retrieves detailed image descriptions and/or additional metadata.

#### Processing & Output

- ResultsProcessor - Implements selection logic for choice of the best POTD.
- ToastNotifications - Manages Windows toast notification displays.
- WindowsRegistry - Facilitates wallpaper updates via Windows Registry.
- RssReader - Specialized module for PODs sourced via RSS feeds.

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
  - __apod__ - NASA APOD open API.
  - __apodweb__ - NASA APOD web-page.
  - __elementy__ - Elementy (science picture of the day).
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

### 5.10.17

- Added `Launch at Startup` menu item.
- The APOD-web pod now supports fetching pictures directly from web pages.
- 1 October 2025: [Astronomy Picture of the Day](https://apod.nasa.gov/apod/) - due to the lapse in federal government funding, NASA is not updating this website.

### 5.8.5-rc1

- Complex refactoring of the inner code base.
- Fixed saving to the directory with the old year in the new year.
- AstroBin (Home of astrophotography) pod was discontinued due to the API being no longer available.

### 4.11.13 🐞

- Fix tray icon mouse click.

### 4.11.12

- Improve validating of wikipedia media types.
- Rebuild mosaic in tray icon on user click (see [tray_icon:mosaic](#application-settings) option).

### 4.9.30 🐞

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
