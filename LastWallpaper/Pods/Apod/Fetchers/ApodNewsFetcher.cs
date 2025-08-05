using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Apod.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Apod.Fetchers;

public sealed class ApodNewsFetcher : INewsFetcher<ApodNews>
{
    public ApodNewsFetcher(
        HttpClient httpClient,
        ApodSettings settings )
    {
        ArgumentNullException.ThrowIfNull( httpClient );
        ArgumentNullException.ThrowIfNull( settings );

        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<Result<ApodNews>> FetchNewsAsync( CancellationToken ct )
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

        ApodImageInfo imageInfo;
        try {
            var info =
                await _httpClient.GetFromJsonAsync<ApodImageInfo>(
                    requestPicturesListUrl,
                    ct );

            if (info is null)
                return Result.Fail( "Empty JSON. No updates were found." );

            imageInfo = info;
        }
        catch (HttpRequestException ex) {
            return Result.Fail(
                $"Failed to fetch JSON data from APOD API: {ex.Message}." );
        }

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

        if (imageDateOk) _lastUpdateDate = DateTime.UtcNow;

        return
            imageDateOk ? Result.Ok(
                new ApodNews() {
                    PodType = PodType.Apod,
                    PubDate = imageDate,
                    Description = imageInfo
                } )
            : Result.Fail(
                $"Can not parse date-time of the picture: {imageInfo.Date}." );
    }

    private DateTime _lastUpdateDate = DateTime.MinValue;

    private readonly ApodSettings _settings;
    private readonly HttpClient _httpClient;

    // Hardcoded latest (today) one image.
    private static readonly CompositeFormat RequestLatestPictureUrlFormat =
        CompositeFormat.Parse( "https://api.nasa.gov/planetary/apod?api_key={0}" );
}
