module Types
open System.Windows.Forms

let inline isNull value =
    obj.ReferenceEquals (value, null)

let inline isNotNull value =
    not (obj.ReferenceEquals (value, null))

type ImageOfTheDay = {
    FileName: string
    Title: string Option
    Description: string Option
    Copyright: string Option
    }

type Msg<'a> =
    | UpdateNow of NotifyIcon
    | QuitApp
    | AboutApp of string