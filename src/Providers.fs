module Providers

open FsHttp
open FSharp.Json
open System.IO
open System

module Bing =
    type ImageInfo = {
        [<JsonField("startdate")>]
        StartDate: string Option

        [<JsonField("urlbase")>]
        UrlBase: string Option

        [<JsonField("copyrighttext")>]
        CopyrightText: string Option

        [<JsonField("title")>]
        Title: string Option
    }

    type BingHpImages = {
        [<JsonField("images")>]
        Images: ImageInfo array
    }

    let bingSrcUrl = // TODO: inject screen size
        $"https://bingwallpaper.microsoft.com/api/BWC/getHPImages?screenWidth={1920}&screenHeight={1080}"

    let saveStreamToFile (s: Stream) =
        let tempImagePath = // TODO: pass image filename
            Path.Combine
                ( Environment.GetFolderPath
                    Environment.SpecialFolder.MyPictures
                , "bingImage.jpg"
                )
        use fs = File.Create(tempImagePath)
        s.CopyTo fs
        s

    let closeStream (s: Stream) = s.Close()

    let loadImage (info: BingHpImages) =
        let iinfo =
            info.Images
            |> Array.item 0

        http {
            GET iinfo.UrlBase.Value
        }
        |> Request.send
        |> Response.toStream
        |> saveStreamToFile
        |> closeStream

    let update () =
        try
            http {
                GET bingSrcUrl
            }
            |> Request.send
            |> Response.toString None
            |> Json.deserialize<BingHpImages>
            |> Some
        with
            _ -> None