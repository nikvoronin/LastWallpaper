module Windows

open Types
open System.Runtime.InteropServices
open Microsoft.Win32;
open Microsoft.Toolkit.Uwp.Notifications;
open System

module Native =
    [<DllImport ("user32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)>]
    extern int SystemParametersInfo (int uAction, int uParam, string lpvParam, int fuWinIni)

module Toast =
    let setupOpt o f (builder: ToastContentBuilder) =
        match o with
        | Some x -> f x builder
        | None -> builder

    let notify f (builder: ToastContentBuilder) =
        builder.Show 
            (fun t -> f t |> ignore)

    let show groupName (info: ImageOfTheDay) =
        (new ToastContentBuilder ())
            .AddHeroImage( new Uri( info.FileName ) )
            |> setupOpt info.Title
                (fun x b -> b.AddText (x))
            |> setupOpt info.Copyright
                (fun x b -> b.AddAttributionText (x)) 
            |> notify
                ( fun t ->
                    t.Group <- groupName
                    t.ExpirationTime <- DateTime.Now.AddDays (2) // TODO: add expiration as option
                )
        
module SysRegistry =
    [<Literal>]
    let SPIF_UPDATEINIFILE: int = 0x01;
    [<Literal>]
    let SPIF_SENDWININICHANGE: int = 0x02;
    [<Literal>]
    let SPI_SETDESKWALLPAPER: int = 20;

    type WallpaperPosition = Center | Tile | Stretch | Span | Fit | Fill

    let strNumOfPos position =
        match position with
        | Center | Tile -> "0"
        | Stretch -> "2"
        | Span -> "22"
        | Fit -> "6"
        | Fill | _ -> "10"

    let strNumOfTiled position =
        match position with
        | Tile -> "1"
        | _ -> "0"

    let setWallpaper (pos: WallpaperPosition ) imagePath =
        use key =
            Registry.CurrentUser
                .OpenSubKey 
                ( @"Control Panel\Desktop"
                , true
                )

        key.SetValue 
            ( @"WallpaperStyle"
            , (strNumOfPos pos)
            )

        key.SetValue 
            ( @"TileWallpaper"
            , (strNumOfTiled pos)
            )

        Native.SystemParametersInfo
            ( SPI_SETDESKWALLPAPER
            , 0
            , imagePath
            , SPIF_UPDATEINIFILE ||| SPIF_SENDWININICHANGE
            ) |> ignore

