open System
open WinFormFs
open System.Windows.Forms
open System.Drawing

[<Literal>]
let AppName = "WinFormFs"
[<Literal>]
let GitHubProjectUrl = "https://github.com/nikvoronin/WinFormFs"

let mainForm =
    let mainMenu =
        Menu.create
            [ "&File"
                |> Menu.strip
                [ "&Open..." |> Menu.stub__TODO
                ; "&Save As..." |> Menu.stub__TODO
                ; Menu.separator ()
                ; "&Quit" |> Menu.verb App.exitA
                ]
            ; "&Edit" |> Menu.stub__TODO
            ; "&View" |> Menu.stub__TODO
            ; "&Help"
                |> Menu.strip
                [ "&Technical Details 🚀"
                    |> Menu.verb
                        (fun _ ->
                            Sys.shellOpen
                                GitHubProjectUrl
                        )
                ; Menu.separator ()
                ; $"&About {AppName}"
                    |> Menu.verb
                        (fun _ ->
                            MessageBox.Show(
                                $"{AppName} v{``WinFormFs Version``}"
                                , $"About {AppName}"
                            ) |> ignore
                        )
                ]
            ]

    let mainStatusLabel =
        StatusBar.label "Ready"

    let mainStatusBar =
        StatusBar.create
            [ mainStatusLabel
                |> setup
                    (fun x ->
                        x.BackColor <-
                            colorFrom KnownColor.LightGreen
                    )
            ; StatusBar.separator ()
            ; StatusBar.progress ()
                |> setup
                    (fun x ->
                        x.Style <- ProgressBarStyle.Marquee
                    )
            ; StatusBar.separator ()
            ; "Want to know more? Select 'Help → ..." |> StatusBar.label
            ]

    "WinForms ♥ F#"
    |> Frm.create
    |> Frm.beginInit
    |> Frm.addControls
        [ Layout.flowV
            [ create<Button>
                (fun btn ->
                    btn.Text <- "Popup Version"
                    //btn.Top <- 0
                    //btn.Left <- 0
                    btn.Click.Add
                        (fun _ ->
                            MessageBox.Show(
                                $"TEST * TEST * TEST"
                                , $"{AppName}"
                            ) |> ignore
                        )
                )
            ; create<Button>
                (fun btn ->
                    btn.Text <- "Disabled"
                    btn.Enabled <- false
                    //btn.Anchor <-
                    //    AnchorStyles.Left 
                    //    ||| AnchorStyles.Bottom
                    btn.Click.Add Stub.doNothingA__TODO
                )
            ; create<Label>
                (fun lbl ->
                    lbl.Text <- "Status color:"
                )
            ; create<ListBox>
                (fun lstBox ->
                    lstBox.Items.AddRange (
                        Array.ofSeq<obj>
                            [ "LightGreen"
                            ; "Magenta"
                            ; "CornflowerBlue"
                            ; "Yellow"
                            ]
                    )

                    lstBox.SelectedIndexChanged.Add (
                        (fun _ ->
                            let itemText = string lstBox.SelectedItem
                            mainStatusLabel.Text <- $" {itemText} "
                            mainStatusLabel.BackColor <-
                                Color.FromName (itemText)
                        )
                    )
                )
            ]
            |> setup
                (fun flwPanel ->
                    flwPanel.Dock <- DockStyle.Fill
                )
        ; mainStatusBar
        ; mainMenu
        ]
    |> Frm.initEnd
    |> setup
        (fun form ->
            form.ClientSize <-
                System.Drawing.Size(640, 400)
        )

let mainNotifyIcon =
    SystemTray.createIcon SystemIcons.Application // TODO: STUB: replace with app-icon
    |> SystemTray.setContextMenu
        ( Menu.createContext
            [ "&Open..." |> Menu.stub__TODO
            ; "&Save As..." |> Menu.stub__TODO
            ; Menu.separator ()
            ; "&Quit" |> Menu.verb App.exitA
            ]
        )
    |> SystemTray.showIcon

[<EntryPoint; STAThread>]
let main argv =
    use _ = mainNotifyIcon

    App.initSysRender
    App.runWith mainForm