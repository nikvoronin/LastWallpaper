using FluentResults;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public sealed class BingPodLoader( HttpClient client )
    : PodLoader( client )
{
    public override string Name => "bing";

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        await using var jsonStream =
            await _client.GetStreamAsync( RequestPicturesList, ct );
        var json = JsonSerializer.Deserialize<BingHpImages>( jsonStream );

        var noUpdates = (json?.Images?.Count ?? 0) == 0;
        if (noUpdates) return Result.Fail( "Empty JSON. No updates were found." );

        var lastImageInfo = json!.Images![0];
        if (lastImageInfo is null) return Result.Fail( "Picture of the day was not found." );

        var urlBase = lastImageInfo.UrlBase;
        if (urlBase is null) return Result.Fail( "Can not find urlbase of the picture." );

        var lastImageUrl =
            string.Format(
                CultureInfo.InvariantCulture,
                DownloadPictureUrlFormat,
                urlBase,
                ImageResolution.UltraHD );

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

        if (!DateTime.TryParseExact(
                lastImageInfo.FullStartDate,
                "yyyyMMddHHmm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fullStartDate ))
            fullStartDate = DateTime.Now;

        (var title,
            var copyrights) = SplitDescription( lastImageInfo.Copyright );

        var result = new Imago() {
            Filename = filename,
            Created = fullStartDate,
            Title = title,
            Copyright = copyrights,
        };

        return Result.Ok( result );
    }

    private static (string? title, string? copyrights) SplitDescription( string? description )
    {
        var descriptionParts = description?.Split( " (©" ) ?? [];
        var hasParts = descriptionParts.Length == 2;
        return
            hasParts ? (descriptionParts[0], $"©{descriptionParts[1][..^1]}")
            : (description, null);
    }

    private static readonly CompositeFormat DownloadPictureUrlFormat =
        CompositeFormat.Parse( "https://www.bing.com{0}_{1}.jpg" );

    // Hardcoded json format, latest (today) zero-indexed one image.
    private const string RequestPicturesList =
        "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

    public static class ImageResolution
    {
        public const string HD = "1280x720";
        public const string FullHD = "1920x1080";
        public const string UltraHD = "UHD";
    }

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