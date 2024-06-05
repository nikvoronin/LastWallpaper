using FluentResults;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public sealed class NasaApodLoader( HttpClient client )
    : PodLoader( client )
{
    public override string Name => "apod";

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        // TODO: move throttling block to the scheduler
        var delta = DateTime.UtcNow - _lastUpdateDate;
        if (delta.TotalHours < 23) {
            return Result.Fail(
                $"Next time. Not now. Last update was {(int)delta.TotalHours} hours ago." );
        }

        var apiKey = DefaultApiKey; // TODO: get from pod configuration or use default one
        var requestPicturesListUrl =
            string.Format(
                CultureInfo.InvariantCulture,
                RequestLatestPictureUrlFormat,
                apiKey );

        var imageInfo =
            await _client.GetFromJsonAsync<ImageInfo>(
                requestPicturesListUrl, ct );

        if (imageInfo is null) return Result.Fail( "Empty JSON. No updates were found." );

        var filename = imageInfo.Date.Replace( "-", "" );

        var imageFilename =
            Path.Combine(
                FileManager.AlbumFolder,
                $"{Name}{filename}.jpeg" );

        // TODO: check here should we load picture or picture is already known
        if (File.Exists( imageFilename )) return Result.Fail( "Picture already known." );

        await using var imageStream =
            await _client.GetStreamAsync( imageInfo.HdImageUrl, ct );

        await using var fileStream =
            new FileStream(
                imageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var wrongDateTimeFormat =
            !DateTime.TryParseExact(
                imageInfo.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime imageDate );
        if (wrongDateTimeFormat)
            imageDate = DateTime.Now;

        // TODO: set time part hour:minute to Now but leave json date unchanged

        var owner =
            imageInfo.Copyright
                ?.Trim().Replace( "\n", "" )
                ?? "(cc) Public domain";

        var result = new Imago() {
            PodName = Name,
            Filename = imageFilename,
            Created = imageDate,
            Title = imageInfo.Title,
            Copyright = owner,
        };

        _lastUpdateDate = DateTime.UtcNow;

        return Result.Ok( result );
    }

    private DateTime _lastUpdateDate = DateTime.MinValue;

    // Hardcoded latest (today) one image.
    private static readonly CompositeFormat RequestLatestPictureUrlFormat =
        CompositeFormat.Parse( "https://api.nasa.gov/planetary/apod?api_key={0}" );

    private const string DefaultApiKey = "DEMO_KEY";

    public class ImageInfo
    {
        [JsonPropertyName( "copyright" )] public string? Copyright { get; init; }
        [JsonPropertyName( "date" )] public required string Date { get; init; }
        [JsonPropertyName( "hdurl" )] public required string HdImageUrl { get; init; }
        [JsonPropertyName( "media_type" )] public required string MediaType { get; init; }
        [JsonPropertyName( "title" )] public required string Title { get; init; }
    }
}