using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing.Models;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing;

public sealed class BingPodLoader(
    HttpClient client,
    IResourceManager resourceManager,
    BingSettings settings )
    : PodLoader( client, resourceManager, settings )
{
    public override BingSettings Settings => (BingSettings)_settings;
    public override string Name => nameof( PodType.Bing ).ToLower();

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
                ImageResolutions.GetValue( Settings.Resolution ) );

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
        else {
            var potdAlreadyKnown =
                _resourceManager.IsPotdAlreadyKnown( Name, startDate );

            if (potdAlreadyKnown)
                return Result.Fail( "Picture already known." );
        }

        await using var imageStream =
            await _client.GetStreamAsync( lastImageUrl, ct );

        var cachedImageFilename =
            _resourceManager.CreateTemporaryCacheFilename();

        await using var fileStream =
            new FileStream(
                cachedImageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        (var title,
            var copyrights) = SplitDescription( lastImageInfo.Copyright );

        var result = new Imago() {
            PodName = Name,
            Filename = cachedImageFilename,
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
}
