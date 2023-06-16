open System
open WinFormFs
open System.Windows.Forms
open System.Drawing

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.16-theta"
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
    // use g = Graphics.FromImage (dst)
    // g.DrawRectangle
    //     ( penWith Color.White// (brightestColor dst)
    //     , 0, 0
    //     , dst.Width - 1
    //     , dst.Height - 1
    //     )

    Icon.FromHandle
        (dst.GetHicon())

let createIconOpt imagePath =
    match imagePath with
    | Some path -> createIconFromImage path
    | None -> SystemIcons.Application

let updateNow (ico: NotifyIcon) =
    async {
        try
            let! x = Providers.Bing.updateAsync ()
            let! imagePath = Providers.Bing.loadImageAsync x

            ico.Icon <- createIconOpt (Some imagePath)
        with _ -> ()
    } |> Async.Start

let updateNowMenuItem =
    "&Update Now"
    |> Menu.stub__TODO

let mainNotifyIcon =
    SystemTray.createIcon
        (createIconOpt None)
    |> SystemTray.setContextMenu
        ( Menu.createContext
            [ updateNowMenuItem
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
    |> SystemTray.showIcon

let initApp () =
    updateNowMenuItem
        .Click.Add
        (fun _ ->
            updateNow mainNotifyIcon
        )

[<EntryPoint; STAThread>]
let main argv =
    use _ = mainNotifyIcon

    initApp ()
    App.run ()
    0
