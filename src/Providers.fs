module Providers

module Bing =
    open FsHttp
    open FSharp.Json
    open System.IO
    open System
    open System.Windows.Forms

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

    let buildBingSourceUrl () =
        let scr = Screen.PrimaryScreen.Bounds.Size
        $"https://bingwallpaper.microsoft.com/api/BWC/getHPImages?screenWidth={scr.Width}&screenHeight={scr.Height}"

    let saveToFileAsync (s: Stream) =
        async {
            let tempImagePath = // TODO: pass image filename
                Path.Combine
                    ( Environment.GetFolderPath
                        Environment.SpecialFolder.MyPictures
                    , "bingImage.jpg"
                    )
            use fs = File.Create(tempImagePath)
            do! s.CopyToAsync fs
                |> Async.AwaitTask

            return tempImagePath
        }

    let loadImageAsync (info: BingHpImages) =
        let iinfo =
            info.Images
            |> Array.item 0

        async {
            let! response =
                http {
                    GET iinfo.UrlBase.Value
                }
                |> Request.sendAsync

            return!
                Response.toStreamAsync response
        }

    let updateAsync () =
        async {
            let! response =
                http {
                    GET (buildBingSourceUrl ())
                }
                |> Request.sendAsync
            
            let! json = Response.toStringAsync None response

            return
                Json.deserialize<BingHpImages> json
        }
