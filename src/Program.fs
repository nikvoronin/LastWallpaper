open System
open WinFormFs
open System.Windows.Forms
open System.Drawing
open Types

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.17-theta"
[<Literal>]
let GitHubProjectUrl = "https://github.com/nikvoronin/LastWallpaper"
let DefaultTrayIconSize = Size (20, 20)

let brightestColor (b: Bitmap) =
    let pixels =
        [ for y in 0 .. b.Height-1 do
            for x in 0 .. b.Width-1 do
                b.GetPixel (x, y)
        ]

    Seq.maxBy
        (fun (x: Color) -> x.GetBrightness ())
        pixels

let penWith (color: Color) =
    new Pen (color)

let createIconFromImage (imagePath: string) =
    use src = new Bitmap (imagePath)
    use dst = new Bitmap (src, DefaultTrayIconSize)

    // TODO: to options, ability to draw border around the tray icon
    use g = Graphics.FromImage (dst)
    g.DrawRectangle
        ( penWith (brightestColor dst) // Color.White// 
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

let updateBingNow (icon: NotifyIcon) =
    async {
        try
            let! x = Providers.Bing.updateAsync ()
            let! imagePath = Providers.Bing.loadImageAsync x

            icon
            |> SystemTray.changeIcon
                (createIconOpt (Some imagePath))
            |> SystemTray.updateText
                $"{AppName}\n{DateTime.Now.ToLongDateString ()} {DateTime.Now.ToLongTimeString ()}" // last update date-time
            |> ignore
        with _ -> ()
    } |> Async.Start

let init () =
    let mainNotifyIcon =
        SystemTray.createIcon (createIconOpt None)

    mainNotifyIcon
    |> SystemTray.setContextMenu
        ( Menu.createContext
            [ "&Update Now"
                |> Menu.verb
                    (fun _ ->
                        updateBingNow mainNotifyIcon
                    )
            ; "&Open Wallpapers Folder" |> Menu.stub__TODO
            ; Menu.separator ()
            ; $"&About {AppName} {AppVersion}"
                |> Menu.verb
                    (fun _ ->
                        Sys.openUrlInBrowser
                            GitHubProjectUrl
                    )
            ; Menu.separator ()
            ; "&Quit" |> Menu.verb App.exitA
            ]
        )
    |> SystemTray.updateText AppName
    |> SystemTray.showIcon

[<EntryPoint; STAThread>]
let main argv =
    use _ = init () // to avoid of disposing the notify icon control
    App.run ()
    0
