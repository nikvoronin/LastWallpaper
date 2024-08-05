using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Nasa.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Nasa;

public sealed class NasaApodLoader(
    HttpClient client,
    IResourceManager resourceManager,
    ApodSettings settings )
    : PodLoader
{
    public override string Name => nameof( PodType.Apod ).ToLower();

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        // TODO:? move throttling block to the scheduler
        var delta = DateTime.UtcNow - _lastUpdateDate;
        if (delta < _settings.ThrottlingHours) {
            return Result.Fail(
                $"Next time. Not now. Last update was {(int)delta.TotalHours} hours ago." );
        }

        var apiKey = _settings.ApiKey;
        var requestPicturesListUrl =
            string.Format(
                CultureInfo.InvariantCulture,
                RequestLatestPictureUrlFormat,
                apiKey );

        var imageInfo =
            await _client.GetFromJsonAsync<ImageInfo>(
                requestPicturesListUrl, ct );

        if (imageInfo is null)
            return Result.Fail( "Empty JSON. No updates were found." );

        if (imageInfo.MediaType != "image")
            return Result.Fail( $"Not an image media type:{imageInfo.MediaType}." );

        var imageDateOk =
            DateTime.TryParseExact(
                imageInfo.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime imageDate );

        if (!imageDateOk) {
            return Result.Fail(
                $"Can not parse date-time of the picture: {imageInfo.Date}." );
        }
        else if (_resourceManager.PotdExists( Name, imageDate ))
            return Result.Fail( "Picture already known." );

        await using var imageStream =
            await _client.GetStreamAsync( imageInfo.HdImageUrl, ct );

        await using var fileStream =
            _resourceManager.CreateTemporaryFileStream();
        var cachedImageFilename = fileStream.Name;

        await imageStream.CopyToAsync( fileStream, ct );

        var owner =
            imageInfo.Copyright
                ?.Trim().Replace( "\n", "" )
                ?? "(cc) Public domain";

        var result = new Imago() {
            PodName = Name,
            Filename = cachedImageFilename,
            Created = imageDate.Date + DateTime.Now.TimeOfDay,
            Title = imageInfo.Title,
            Copyright = owner,
        };

        _lastUpdateDate = DateTime.UtcNow;

        return Result.Ok( result );
    }

    private DateTime _lastUpdateDate = DateTime.MinValue;
    private readonly ApodSettings _settings = settings;
    private readonly HttpClient _client = client;
    private readonly IResourceManager _resourceManager = resourceManager;

    // Hardcoded latest (today) one image.
    private static readonly CompositeFormat RequestLatestPictureUrlFormat =
        CompositeFormat.Parse( "https://api.nasa.gov/planetary/apod?api_key={0}" );
}
