open System
open WinFormFs
open System.Windows.Forms
open System.Drawing

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.13-temp"
[<Literal>]
let GitHubProjectUrl = "https://github.com/nikvoronin/LastWallpaper"
let DefaultTrayIconSize = Size (20, 20)

let createIconFromImage (imagePath: string) =
    let src = new Bitmap (imagePath)
    let dst = new Bitmap (src, DefaultTrayIconSize)
    let g = Graphics.FromImage (dst)

    g.DrawRectangle
        ( Pens.White
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

let updateNow (ico: NotifyIcon) =
    async {
        try
            let! x = Providers.Bing.updateAsync ()
            use! fs = Providers.Bing.loadImageAsync x
            let! icoPath =
                Providers.Bing.saveToFileAsync fs
            fs.Close ()

            ico.Icon <- createIconOpt (Some icoPath)
        with _ -> ()
    } |> Async.Start

let updateNowMenuItem =
    "&Update Now"
    |> Menu.stub__TODO

let mainNotifyIcon =
    SystemTray.createIcon
        (createIconOpt None) // TODO: STUB: replace with proper icon
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
