using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing.Models;
using LastWallpaper.Pods.Elementy.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing;

public sealed class BingPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager,
    BingSettings settings )
    : HttpPodLoader<BingPodNews>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Bing ).ToLower();

    protected async override Task<Result<BingPodNews>> FetchNewsInternalAsync(
        CancellationToken ct )
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

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        BingPodNews news, CancellationToken ct )
    {
        var cachedFilenameResult =
            await DownloadFileAsync( news.LastImageUrl, ct );

        if (cachedFilenameResult.IsFailed)
            return Result.Fail(
                $"Can not download media from {news.LastImageUrl}." );

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedFilenameResult.Value,
            Created = news.PubDate,
            Title = news.Title,
            Copyright = news.Copyright,
        };

        return Result.Ok( result );
    }

    private static readonly CompositeFormat DownloadPictureUrlFormat =
        CompositeFormat.Parse( "https://www.bing.com{0}_{1}.jpg" );

    private readonly BingSettings _settings = settings;

    // Hardcoded json format, latest (today) zero-indexed one image.
    private const string RequestPicturesList =
        "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";
}
