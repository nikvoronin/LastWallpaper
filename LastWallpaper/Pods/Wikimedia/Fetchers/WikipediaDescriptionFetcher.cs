using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Wikimedia.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Wikimedia.Fetchers;

public sealed class WikipediaDescriptionFetcher : IDescriptionFetcher<WikipediaPodNews>
{
    public WikipediaDescriptionFetcher( HttpClient httpClient )
    {
        ArgumentNullException.ThrowIfNull( httpClient );

        _httpClient = httpClient;
    }

    public async Task<Result<PotdDescription>> FetchDescriptionAsync(
        WikipediaPodNews news,
        CancellationToken ct )
    {
        var potdFilename = news.Response.Query.Pages[0].Images?[0].Value;

        var jsonPotdImageLink =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryTemplates.PotdUrlFormat,
                    potdFilename ),
                ct );

        var potdImageDownloadLink = jsonPotdImageLink?.Query.Pages[0].ImageInfos?[0].Url;

        if (potdImageDownloadLink is null)
            return Result.Fail( "No image url were found." );

        var jsonPotdCredits =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryTemplates.PotdCreditsFormat,
                    potdFilename ),
                ct );

        // TODO: make it safe, should control nullable values
        var extMetaData = jsonPotdCredits?.Query.Pages[0].ImageInfos[0].ExtMetaData;

        var objectName = extMetaData.ObjectName();
        var fullDescription = extMetaData.ImageDescription();

        var description = fullDescription;
        if (description?.Length > 200) {
            var dotIndex = description.IndexOf( ". " );
            if (dotIndex > -1)
                description = description[..dotIndex];
        }

        var artist = extMetaData.Artist();
        var credit = extMetaData.Credit();

        var title =
            description.Length < 200 ? description
            : objectName;
        var copyrights =
            artist.Length + credit.Length > 100 ? artist
            : $"{artist}/{credit}";

        return Result.Ok(
            new PotdDescription() {
                PodType = PodType.Wikipedia,
                Url = new Uri( potdImageDownloadLink ),
                PubDate = DateTime.Now,
                Title = title,
                Copyright = copyrights,
                Description = fullDescription
            } );
    }

    private readonly HttpClient _httpClient;
}
