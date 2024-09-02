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
    : HttpPodLoader<WikipediaPodNews>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Wikipedia ).ToLower();

    protected async override Task<Result<WikipediaPodNews>> FetchNewsInternalAsync(
        CancellationToken ct )
    {
        var nowDate = DateTime.Now.Date;

        var jsonPotdFilename =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdFilenameFormat,
                    nowDate.ToString( "yyyy-MM-dd" ) ),
                ct );

        if (jsonPotdFilename is null
            || (jsonPotdFilename.Query.Pages[0].Missing ?? false))
            return Result.Fail( $"No updates for {nowDate}" );

        return Result.Ok(
            new WikipediaPodNews() {
                PubDate = nowDate,
                Response = jsonPotdFilename
            } );

    }

    protected async override Task<Result<PotdDescription>> GetDescriptionAsync(
        WikipediaPodNews news,
        CancellationToken ct )
    {
        var potdFilename = news.Response.Query.Pages[0].Images?[0].Value;

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

        return Result.Ok(
            new PotdDescription() {
                Url = new Uri( potdImageDownloadLink ),
                PubDate = DateTime.Now,
                Title = title,
                Copyright = copyrights,
                Description = fullDescription
            } );
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
