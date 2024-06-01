using FluentResults;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public sealed class BingMay24 : PictureDayLoader
{
    public override string Name => "bing";

    public BingMay24( HttpClient client )
        : base( client )
    { }

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        await using var jsonStream =
            await _client.GetStreamAsync( RequestPicturesList, ct );
        var json = JsonSerializer.Deserialize<BingHpImages>( jsonStream );

        var noUpdates = (json?.Images?.Count ?? 0) == 0;
        if (noUpdates) return Result.Fail( "Empty JSON. No updates were found." );

        var lastImageUrl = $"https://www.bing.com{json!.Images![0]!.UrlBase}_UHD.jpg";

        await using var imageStream =
            await _client.GetStreamAsync( lastImageUrl, ct );

        // TODO: use cache folder instead
        var folder = Environment.GetFolderPath(
            Environment.SpecialFolder.MyPictures );
        var filename =
            Path.Combine(
                folder,
                $"{Guid.NewGuid()}.jpeg" );

        await using var fileStream =
            new FileStream(
                filename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var imageDateTime =
            DateTime.ParseExact(
                json!.Images![0]!.FullStartDate!,
                "yyyyMMddHHmm",
                CultureInfo.InvariantCulture );

        // TODO: parse then split copyrights into title+copyright pair
        var description = json!.Images![0]!.Copyright;
        var titleCopyrights = description!.Split( " (©" );
        var title = titleCopyrights[0];
        var copyrights = $"©{titleCopyrights[1][..^1]}";

        var result = new Imago() {
            Filename = filename,
            Created = imageDateTime,
            Title = title,
            Copyright = copyrights,
        };

        return Result.Ok( result );
    }

    private const string RequestPicturesList =
        "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

    public class ImageInfo
    {
        [JsonPropertyName( "fullstartdate" )]
        public string? FullStartDate { get; set; }

        [JsonPropertyName( "urlbase" )]
        public string? UrlBase { get; set; }

        [JsonPropertyName( "copyright" )]
        public string? Copyright { get; set; }

        [JsonPropertyName( "title" )]
        public string? Title { get; set; }
    }

    public class BingHpImages
    {
        [JsonPropertyName( "images" )]
        public IReadOnlyList<ImageInfo>? Images { get; set; }
    }
}