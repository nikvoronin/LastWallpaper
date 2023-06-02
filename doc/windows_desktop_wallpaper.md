# Set Windows Desktop Background

Registry key: `HKEY_CURRENT_USER\Control Panel\Desktop\`

There are three values of type `REG_SZ` (i.e. string type):

- `WallPaper` - picture filename with a full path. For ex.: `c:\user\documents\my pictures\wallpaper.jpeg`
- `WallpaperStyle` - Specifies how the desktop wallpaper should be displayed:
    - `0` - Center the image; do not stretch.
    - `0` - Tile the image across all monitors.
    - `2` - Stretch the image to exactly fit on the monitor, without maintain aspect ratio.
    - `6` - Stretch the image to exactly the height or width of the monitor without changing its aspect ratio or cropping the image. This can result in colored letterbox bars on either side or on above and below of the image.
    - `10` - Stretch the image to fill the screen, cropping the image as necessary to avoid letterbox bars. Often used as default.
    - `22` - Spans a single image across all monitors attached to the system.
- `TileWallpaper` - tile on/off
    - `0` - tile off
    - `1` - tile wallpaper

See also:

- [Set Desktop Background](https://codereview.stackexchange.com/questions/60755/set-desktop-background). /StackExchange
- [Change Desktop Background](https://codereview.stackexchange.com/questions/71730/change-desktop-background). /StackExchange
