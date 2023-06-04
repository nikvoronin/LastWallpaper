open System
open WinFormFs
open System.Windows.Forms
open System.Drawing

[<Literal>]
let AppName = "The Last Wallpaper"
[<Literal>]
let AppVersion = "3.6.4"
[<Literal>]
let GitHubProjectUrl = "https://github.com/nikvoronin/LastWallpaper"

let mainNotifyIcon =
    SystemTray.createIcon SystemIcons.Application // TODO: STUB: replace with proper icon
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
    App.run () |> ignore
    0
