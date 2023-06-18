open System
open WinFormFs
open System.Windows.Forms
open System.Drawing
open Types

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.18-alpha"
[<Literal>]
let GitHubProjectUrl = "https://github.com/nikvoronin/LastWallpaper"
let DefaultTrayIconSize = Size (20, 20)

let findBrightestColor (b: Bitmap) =
    Seq.maxBy
        (fun (x: Color) -> x.GetBrightness ())
        [ for y in 0 .. b.Height-1 do
            for x in 0 .. b.Width-1 do
                b.GetPixel (x, y)
        ]

let penWith (color: Color) =
    new Pen (color)

let createIconFromImage (imagePath: string) =
    use src = new Bitmap (imagePath)
    use dst =
        new Bitmap
            ( DefaultTrayIconSize.Width
            , DefaultTrayIconSize.Height
            )

    // TODO: to options, ability to draw border around the tray icon
    use g = Graphics.FromImage (dst)
    g.InterpolationMode <-
        Drawing2D.InterpolationMode.NearestNeighbor
    g.DrawImage
        ( src
        , 0, 0
        , dst.Width
        , dst.Height
        )

    use pen = penWith (findBrightestColor dst)
    g.DrawRectangle
        ( pen
        , 0, 0
        , dst.Width - 1
        , dst.Height - 1
        )

    Icon.FromHandle
        (dst.GetHicon())

let createIconOpt imagePath =
    match imagePath with
    | Some path -> createIconFromImage path
    | None -> SystemIcons.Application

let updateNotifyIcon (notifyIcon: NotifyIcon) imagePath =
        notifyIcon
        |> SystemTray.changeIcon
            (createIconOpt (Some imagePath))
        |> SystemTray.updateText
            $"{AppName}\n{DateTime.Now.ToLongDateString ()} {DateTime.Now.ToLongTimeString ()}" // last update date-time
        |> ignore

let updateBingNow (icon: NotifyIcon) =
    async {
        try
            let! x = Providers.Bing.updateAsync ()
            let! result = Providers.Bing.loadImageAsync x

            match result with
            | FreshImage path ->
                Windows.SysRegistry.setWallpaper
                    Windows.SysRegistry.Fill
                    path
                updateNotifyIcon icon path
            | _ -> ()
        with _ -> ()
    } |> Async.Start

let dispatch msg =
    match msg with
    | UpdateNow x -> updateBingNow x
    | QuitApp -> App.exit ()
    | AboutApp url -> Sys.shellOpen url
    | ExploreFolder path -> Sys.shellOpen path

let init send =
    let mainNotifyIcon =
        SystemTray.createIcon (createIconOpt None) // TODO: try to open the last known image at the start

    mainNotifyIcon
    |> SystemTray.setContextMenu
        ( Menu.createContext
            [ "Update Now"
                |> Menu.verb
                    (fun _ -> send (Msg.UpdateNow mainNotifyIcon))
            ; "Explore Wallpapers Folder"
                |> Menu.verb
                    (fun _ -> send (Msg.ExploreFolder Providers.Bing.getImageFolderBase))
            ; Menu.separator ()
            ; $"About v{AppVersion}"
                |> Menu.verb
                    (fun _ -> send (Msg.AboutApp GitHubProjectUrl))
            ; Menu.separator ()
            ; "Quit"
                |> Menu.verb 
                    (fun _ -> send Msg.QuitApp)
            ]
        )
    |> SystemTray.updateText AppName
    |> SystemTray.showIcon

[<EntryPoint; STAThread>]
let main argv =
    use ni = init dispatch // to avoid of disposing the notify icon control
    use t =
        Timer.startImmediate
            (TimeSpan.FromHours (1))
            (fun () -> dispatch (Msg.UpdateNow ni))

    App.run ()
    Timer.stop t
    0
