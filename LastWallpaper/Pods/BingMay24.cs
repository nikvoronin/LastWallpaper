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
    public override string Name => nameof( BingMay24 );

    public BingMay24( HttpClient client )
        : base( client )
    { }

    protected override async Task<IReadOnlyCollection<string>> UpdateInternalAsync(
        CancellationToken ct )
    {
        await using var jsonStream =
            await _client.GetStreamAsync( RequestPicturesList, ct );
        var json = JsonSerializer.Deserialize<BingHpImages>( jsonStream );

        var noUpdates = (json?.Images?.Count ?? 0) == 0;
        if (noUpdates) return [];

        var lastImageUrl = $"https://www.bing.com{json!.Images![0]!.UrlBase}_UHD.jpg";

        await using var imageStream =
            await _client.GetStreamAsync( lastImageUrl, ct );

        var filename = $"{Guid.NewGuid()}.jpeg";

        await using var fileStream =
            new FileStream(
                filename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        //var imageDateTime =
        //    DateTime.ParseExact(
        //        json!.Images![0]!.FullStartDate!,
        //        "yyyyMMddHHmm",
        //        CultureInfo.InvariantCulture );

        return [filename];
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