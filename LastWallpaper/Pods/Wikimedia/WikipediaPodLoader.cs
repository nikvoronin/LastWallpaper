using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Wikimedia.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Wikimedia;

public sealed class WikipediaPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HttpPodLoader( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Wikipedia ).ToLower();

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        CancellationToken ct )
    {
        var imageDate = DateTime.Now;

        if (_resourceManager.PotdExists( Name, imageDate ))
            return Result.Fail( "Picture already known." );

        var jsonPotdFilename =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdFilenameFormat,
                    DateTime.Now.Date.ToString( "yyyy-MM-dd" ) ),
                ct );

        var potdFilename = jsonPotdFilename?.Query.Pages[0].Images?[0].Value;

        var jsonPotdImageLink =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdUrlFormat,
                    potdFilename ),
                ct );

        var potdImageDownloadLink = jsonPotdImageLink?.Query.Pages[0].ImageInfos?[0].Url;

        if (potdImageDownloadLink is null)
            return Result.Fail( "No image url were found." );

        var cachedFilenameResult =
            await DownloadFileAsync( potdImageDownloadLink, ct );

        if (cachedFilenameResult.IsFailed)
            return Result.Fail(
                $"Can not download media from {potdImageDownloadLink}." );

        var jsonPotdCredits =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdCreditsFormat,
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

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedFilenameResult.Value,
            Created = DateTime.Now,
            Title = title,
            Copyright = copyrights,
            Description = fullDescription
        };

        return Result.Ok( result );
    }

    private static readonly CompositeFormat WmQueryPotdFilenameFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=images&titles=Template:POTD/{0}" );

    private static readonly CompositeFormat WmQueryPotdUrlFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=imageinfo&iiprop=url&titles={0}" );

    private static readonly CompositeFormat WmQueryPotdCreditsFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=imageinfo&iiprop=extmetadata&titles={0}" );

    private const string WikiMediaQueryBase =
        "https://en.wikipedia.org/w/api.php?action=query&format=json&formatversion=2";
}
