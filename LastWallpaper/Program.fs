open System
open WinFormFs
open System.Windows.Forms
open System.Drawing

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.6"
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

let mainNotifyIcon =
    SystemTray.createIcon
        (createIconOpt None) // TODO: STUB: replace with proper icon
    |> SystemTray.setContextMenu
        ( Menu.createContext
            [ "&Update Now" |> Menu.stub__TODO
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

[<EntryPoint; STAThread>]
let main argv =
    use _ = mainNotifyIcon
    App.run ()
    0
