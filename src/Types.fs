module Types
open System.Windows.Forms

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
    | ExploreFolder of string

type UpdateResult =
    | FreshImage of string
    | Actual of string
