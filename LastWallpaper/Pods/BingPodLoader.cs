using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing;

public sealed class BingPodLoader(
    HttpClient client,
    BingSettings settings )
    : PodLoader( PodType.Bing, client, settings )
{
    public override BingSettings Settings => (BingSettings)_settings;

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        var json =
            await _client.GetFromJsonAsync<BingHpImages>(
                RequestPicturesList, ct );

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
                Settings.Resolution );

        var imageFilename =
            Path.Combine(
                FileManager.AlbumFolder,
                $"{Name}{lastImageInfo.StartDate}.jpeg" );

        // TODO: check here should we load picture or picture is already known
        if (File.Exists( imageFilename )) return Result.Fail( "Picture already known." );

        await using var imageStream =
            await _client.GetStreamAsync( lastImageUrl, ct );

        await using var fileStream =
            new FileStream(
                imageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var wrongDateTimeFormat =
            !DateTime.TryParseExact(
                lastImageInfo.StartDate,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime startDate );
        if (wrongDateTimeFormat)
            startDate = DateTime.Now;

        (var title,
            var copyrights) = SplitDescription( lastImageInfo.Copyright );

        var result = new Imago() {
            PodName = Name,
            Filename = imageFilename,
            Created = startDate.Date + DateTime.Now.TimeOfDay,
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
        [JsonPropertyName( "startdate" )] public required string StartDate { get; init; }

        [JsonPropertyName( "urlbase" )] public required string UrlBase { get; init; }

        [JsonPropertyName( "copyright" )] public string? Copyright { get; set; }
    }

    public class BingHpImages
    {
        [JsonPropertyName( "images" )]
        public IReadOnlyList<ImageInfo>? Images { get; set; }
    }
}

public class BingSettings : IPotdLoaderSettings
{
    [JsonPropertyName( "resolution" )]
    public string Resolution { get; init; } =
        BingPodLoader.ImageResolution.UltraHD;
}
