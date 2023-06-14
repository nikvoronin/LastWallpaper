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

    let getImageFolderBase =
        Path.Combine
            ( Environment.GetFolderPath
                Environment.SpecialFolder.MyPictures
            , "The Last Wallpaper"
            )

    let getOrCreateFolder () =
        let folder =
                Path.Combine
                    ( getImageFolderBase
                    , DateTime.Now.Year.ToString ()
                    )
        
        if not (Directory.Exists folder) then
            Directory.CreateDirectory (folder) |> ignore
        
        folder

    let loadImageAsync (info: BingHpImages) =
        let iinfo =
            info.Images
            |> Array.item 0

        let name =
            match iinfo.StartDate with
            | Some x -> x
            | _ -> $"{DateTime.Now:yyyyMMdd}"

        let filename =
            Path.Combine
                ( getOrCreateFolder ()
                , $"bing{name}.jpg"
                )

        async {
            if not (File.Exists filename) then
                let! response =
                    http {
                        GET iinfo.UrlBase.Value
                    }
                    |> Request.sendAsync

                use! s = Response.toStreamAsync response

                use fs = File.Create(filename)
                do! s.CopyToAsync fs
                    |> Async.AwaitTask
            
            return filename
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
