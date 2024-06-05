﻿using FluentResults;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
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

        var lastImageInfo =
            await _client.GetFromJsonAsync<ImageInfo>(
                RequestPicturesList, ct );

        if (lastImageInfo is null) return Result.Fail( "Empty JSON. No updates were found." );

        var filename = lastImageInfo.Date.Replace( "-", "" );

        var imageFilename =
            Path.Combine(
                FileManager.AlbumFolder,
                $"{Name}{filename}.jpeg" );

        // TODO: check here should we load picture or picture is already known
        if (File.Exists( imageFilename )) return Result.Fail( "Picture already known." );

        await using var imageStream =
            await _client.GetStreamAsync( lastImageInfo.HdImageUrl, ct );

        await using var fileStream =
            new FileStream(
                imageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var wrongDateTimeFormat =
            !DateTime.TryParseExact(
                lastImageInfo.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime imageDate );
        if (wrongDateTimeFormat)
            imageDate = DateTime.Now;

        var copyrights =
            lastImageInfo.Copyright
                ?.Trim().Replace( "\n", "" );

        var result = new Imago() {
            PodName = Name,
            Filename = imageFilename,
            Created = imageDate,
            Title = lastImageInfo.Title,
            Copyright = copyrights,
        };

        _lastUpdateDate = DateTime.UtcNow;

        return Result.Ok( result );
    }

    private DateTime _lastUpdateDate = DateTime.MinValue;

    // Hardcoded latest (today) one image.
    private const string RequestPicturesList =
        "https://api.nasa.gov/planetary/apod?api_key=DEMO_KEY";

    public class ImageInfo
    {
        [JsonPropertyName( "copyright" )] public string? Copyright { get; init; }
        [JsonPropertyName( "date" )] public required string Date { get; init; }
        [JsonPropertyName( "hdurl" )] public required string HdImageUrl { get; init; }
        [JsonPropertyName( "media_type" )] public required string MediaType { get; init; }
        [JsonPropertyName( "title" )] public required string Title { get; init; }
    }
}