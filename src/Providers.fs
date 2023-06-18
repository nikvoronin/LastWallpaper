module Providers

module Bing =
    open FsHttp
    open FSharp.Json
    open System.IO
    open System
    open System.Windows.Forms
    open System.Globalization

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

    let extractHint (x: ImageInfo) =
        let title =
            if x.Title.IsSome
            then $"{x.Title.Value}\n"
            else String.Empty
        let dt =
            match x.StartDate with
            | Some d ->
                (DateTime
                    .ParseExact (d, "yyyyMMdd", CultureInfo.InvariantCulture))
                    .ToLongDateString ()
            | _ -> String.Empty

        $"{title}{dt}"

    let buildBingSourceUrl () =
        let scr = Screen.PrimaryScreen.Bounds.Size
        "https://bingwallpaper.microsoft.com/api/BWC/getHPImages"
        + $"?screenWidth={scr.Width}&screenHeight={scr.Height}"

    let getImageFolderBase =
        // TODO: add option to override folder through application settings file
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
        Directory.CreateDirectory folder
        |> ignore
        
        folder

    let downloadImageAsync url filename =
        async {
            let! response =
                http {
                    GET url
                }
                |> Request.sendAsync

            use! s = Response.toStreamAsync response
            use fs = File.Create filename

            do!
                s.CopyToAsync fs
                |> Async.AwaitTask
        }

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
            if File.Exists filename then
                return Types.Actual filename
            else
                do! downloadImageAsync
                        iinfo.UrlBase.Value
                        filename

                return Types.FreshImage filename
        }

    let updateAsync () =
        async {
            let! response =
                http {
                    GET (buildBingSourceUrl ())
                }
                |> Request.sendAsync
            
            let! json =
                Response.toStringAsync None response

            return
                Json.deserialize<BingHpImages> json
        }
