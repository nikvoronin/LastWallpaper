using FluentResults;
using LastWallpaper.Models;
using LastWallpaper.Pods.Wikimedia.Models;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Wikimedia;

public sealed class WikipediaPodLoader(
    HttpClient client,
    WikipediaSettings settings )
    : PodLoader( PodType.Wikipedia, client, settings )
{
    public override WikipediaSettings Settings => (WikipediaSettings)_settings;

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        var jsonPotdFilename =
            await _client.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdFilenameFormat,
                    DateTime.Now.Date.ToString( "yyyy-MM-dd" ) ),
                ct );

        var potdFilename = jsonPotdFilename?.Query.Pages[0].Images?[0].Value;

        var jsonPotdImageLink =
            await _client.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdUrlFormat,
                    potdFilename ),
                ct );

        var potdImageDownloadLink = jsonPotdImageLink?.Query.Pages[0].ImageInfos?[0].Url;

        if (potdImageDownloadLink is null) return Result.Fail( "No image url were found." );

        await using var imageStream =
            await _client.GetStreamAsync( potdImageDownloadLink, ct );

        var filename = $"{Name}{DateTime.Now:yyyyMMdd}.jpeg";

        var imageFilename =
            Path.Combine(
                FileManager.AlbumFolder,
                filename );

        // TODO: check here should we load picture or picture is already known
        if (File.Exists( imageFilename )) return Result.Fail( "Picture already known." );

        await using var fileStream =
            new FileStream(
                imageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var jsonPotdCredits =
            await _client.GetFromJsonAsync<WmResponse>(
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

        var result = new Imago() {
            PodName = Name,
            Filename = imageFilename,
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
