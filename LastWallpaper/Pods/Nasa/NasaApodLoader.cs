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
    HttpClient httpClient,
    IResourceManager resourceManager,
    ApodSettings settings )
    : HttpPodLoader<NasaApodNews>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Apod ).ToLower();

    protected async override Task<Result<NasaApodNews>> FetchNewsInternalAsync(
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
            await _httpClient.GetFromJsonAsync<ImageInfo>(
                requestPicturesListUrl, ct );

        if (imageInfo is null)
            return Result.Fail( "Empty JSON. No updates were found." );

        var notAnImage =
            imageInfo.MediaType != "image"
            || string.IsNullOrWhiteSpace( imageInfo.HdImageUrl );
        if (notAnImage)
            return Result.Fail(
                $"Not an image media type:{imageInfo.MediaType}." );

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

        return Result.Ok(
            new NasaApodNews() {
                PubDate = imageDate,
                Description = imageInfo
            } );
    }

    protected override Task<Result<PotdDescription>> GetDescriptionAsync(
        NasaApodNews news,
        CancellationToken ct )
    {
        var imageInfo = news.Description;

        var owner =
            imageInfo.Copyright
                ?.Trim().Replace( "\n", "" )
                ?? "(cc) Public domain";

        _lastUpdateDate = DateTime.UtcNow;

        return Task.FromResult( Result.Ok(
            new PotdDescription() {
                Url = new Uri( imageInfo.HdImageUrl ),
                PubDate = news.PubDate,
                Title = imageInfo.Title,
                Copyright = owner,
            } ) );
    }

    private DateTime _lastUpdateDate = DateTime.MinValue;
    private readonly ApodSettings _settings = settings;

    // Hardcoded latest (today) one image.
    private static readonly CompositeFormat RequestLatestPictureUrlFormat =
        CompositeFormat.Parse( "https://api.nasa.gov/planetary/apod?api_key={0}" );
}
