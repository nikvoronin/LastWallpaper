module WinFormFs

open System.Windows.Forms
open System.Drawing
open System.Diagnostics

[<Literal>]
let ``WinFormFs Version`` = "3.6.18-alpha"

let colorFrom c =
    Color.FromKnownColor c

let setup g (x: 'a) : 'a =
    g x
    x

let create<'a when 'a : (new : unit -> 'a)> g =
    new 'a ()
    |> setup g


module App =
    /// Configure HighDpi scalling for looking good
    let initSysRender =
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        Application.SetHighDpiMode(HighDpiMode.SystemAware) |> ignore

    /// Starts application with given form
    let runWith (form: Form) =
        Application.Run(form)
        0

    /// Starts window-less application 
    let run () =
        Application.Run()

    let exit () = Application.Exit()
    let exitA _ = Application.Exit()


module Frm =
    let addControls (controls: Control seq) (form: Form) =
        form.Controls.AddRange( Array.ofSeq controls)
        form

    let beginInit (form: Form) =
        form.SuspendLayout()
        form.AutoScaleDimensions <- new System.Drawing.SizeF(6F, 13F)
        form.AutoScaleMode <- AutoScaleMode.Font
        form

    let initEnd (form: Form) =
        form.ResumeLayout(false)
        form.PerformLayout()
        form

    let create (caption: string) =
        let form = new Form()
        form.Text <- caption
        form


module ToolStrip =
    let separator () =
        new ToolStripSeparator()


module Menu =
    let separator = ToolStrip.separator

    let verb click (text: string) : ToolStripItem =
        let item = new ToolStripMenuItem(text)
        item.Click.Add( click )
        item

    let stub__TODO (text: string) =
        let root = new ToolStripMenuItem(text)
        root

    let strip (items: ToolStripItem seq) (text: string) : ToolStripItem =
        let root = new ToolStripMenuItem(text)
        root.DropDownItems.AddRange(Array.ofSeq items)
        root

    let create (items: ToolStripItem seq) =
        let menu = new MenuStrip()
        menu.Items.AddRange (Array.ofSeq items) |> ignore
        menu

    let createContext (items: ToolStripItem seq) =
        let menu = new ContextMenuStrip()
        menu.Items.AddRange (Array.ofSeq items) |> ignore
        menu


module Sys =
    /// Open URL in a system web browser. Call cmd start
    let shellOpen path =
        ProcessStartInfo 
            ( FileName = path
            , CreateNoWindow = true
            , UseShellExecute = true
            )
        |> Process.Start
        |> ignore

module Stub =
    /// Just a stub for event handlers. Lets do nothing
    let doNothingA__TODO = fun x -> ()


module StatusBar =
    let separator = ToolStrip.separator

    let create (items: ToolStripItem seq) =
        let bar = new StatusStrip()
        bar.Items.AddRange(Array.ofSeq items)
        bar

    let label (text: string) =
        let label = new ToolStripLabel(text)
        label

    let progress () =
        let bar = new ToolStripProgressBar()
        bar


module Layout =
    let panel (controls: Control seq) =
        let panel = new Panel()
        panel.Controls.AddRange(Array.ofSeq controls)
        panel

    let flowV (controls: Control seq) =
        let panel = new FlowLayoutPanel()
        panel.FlowDirection <- FlowDirection.TopDown
        panel.Controls.AddRange(Array.ofSeq controls)
        panel
    let stackV = flowV

    let flowH (controls: Control seq) =
        let panel = new FlowLayoutPanel()
        panel.FlowDirection <- FlowDirection.LeftToRight
        panel.Controls.AddRange(Array.ofSeq controls)
        panel
    let stackH = flowH

module SystemTray =
    let createIcon (icon: Icon) =
        let notifyIcon = new NotifyIcon ()
        notifyIcon.Icon <- icon
        notifyIcon

    let setContextMenu menu (notifyIcon: NotifyIcon) =
        notifyIcon.ContextMenuStrip <- menu
        notifyIcon

    let updateText hintText (notifyIcon: NotifyIcon) =
        notifyIcon.Text <- hintText
        notifyIcon

    let changeIcon icon (notifyIcon: NotifyIcon) =
        notifyIcon.Icon <- icon
        notifyIcon

    let showIcon (notifyIcon: NotifyIcon) =
        notifyIcon.Visible <- true
        notifyIcon

    let hideIcon (notifyIcon: NotifyIcon) =
        notifyIcon.Visible <- false
        notifyIcon