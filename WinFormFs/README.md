# WinFormFs

A subtle wrapper to convenient use of Windows Forms in F#

## Features

- Application w-or-without windows
- Windows (Form)
- Menus
    - Subitems
    - Separator
    - Popup menus (ContextMenu)
    - App main menu (MenuStrip)
- System operations
    - Open URL in system browser
- StatusBar (StatusStrip)
    - Label
    - ProgressBar
    - Separator
- Controls (any descendants of the Control class)
- Layouts
    - Panel
    - Flow Layouts (or StackPanel in WPF terms), both vertical and/or horizontal
- System tray (NotifyIcon)

## Release Notes

```fsharp
printfn $"{``WinFormFs Version``}"
```

### 3.5.xx-alpha

- Add generic `create<Control>` which can create any control. Example of creating `ListBox` control
- Add flow layout panels aka stackPanel (both horizontal and vertical).
- Add `ProgressBar` for `StatusBar`.
- `Do` style `create` and `setup` to fine tuning descendants of the `Control` class.
- System tray (NotifyIcon).
- Changed order of arguments, it starts from title now: `"Click me" |> StatusBar.label`.
