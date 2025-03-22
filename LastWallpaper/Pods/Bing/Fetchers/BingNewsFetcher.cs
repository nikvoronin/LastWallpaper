using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing.Fetchers;

public sealed class BingNewsFetcher : INewsFetcher<BingPodNews>
{
    public BingNewsFetcher(
        HttpClient httpClient,
        BingSettings settings )
    {
        ArgumentNullException.ThrowIfNull( httpClient );
        ArgumentNullException.ThrowIfNull( settings );

        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<Result<BingPodNews>> FetchNewsAsync( CancellationToken ct )
    {
        var json =
            await _httpClient.GetFromJsonAsync<BingHpImages>(
                RequestPicturesList, ct );

        var noUpdates = (json?.Images?.Count ?? 0) == 0;
        if (noUpdates) return Result.Fail( "Empty JSON. No updates were found." );

        var lastImageInfo = json!.Images![0];
        if (lastImageInfo is null) return Result.Fail( "Picture of the day was not found." );

        (var title,
            var copyrights) = SplitDescription( lastImageInfo.Copyright );

        var urlBase = lastImageInfo.UrlBase;
        if (urlBase is null) return Result.Fail( "Can not find urlbase of the picture." );

        var lastImageUrl =
            string.Format(
                CultureInfo.InvariantCulture,
                DownloadPictureUrlFormat,
                urlBase,
                ImageResolutions.GetValue( _settings.Resolution ) );

        var startDateOk =
            DateTime.TryParseExact(
                lastImageInfo.StartDate,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime startDate );

        if (!startDateOk) {
            return Result.Fail(
                $"Can not parse date-time of the picture: {lastImageInfo.StartDate}." );
        }

        return Result.Ok(
            new BingPodNews() {
                PodType = PodType.Bing,
                PubDate = startDate,
                LastImageUrl = lastImageUrl,
                Copyright = copyrights,
                Title = title,
            } );

        static (string? title, string? copyrights) SplitDescription( string? description )
        {
            var descriptionParts = description?.Split( " (©" ) ?? [];
            var hasParts = descriptionParts.Length == 2;

            return
                hasParts ? (descriptionParts[0], $"©{descriptionParts[1][..^1]}")
                : (description, null);
        }
    }

    private static readonly CompositeFormat DownloadPictureUrlFormat =
        CompositeFormat.Parse( "https://www.bing.com{0}_{1}.jpg" );

    private readonly BingSettings _settings;
    private readonly HttpClient _httpClient;

    // Hardcoded json format, latest (today) zero-indexed one image.
    private const string RequestPicturesList =
        "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";
}
