using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Copernicus.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Copernicus;

public sealed class CopernicusPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HttpPodLoader<CopernicusPodNews>( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Copernicus ).ToLower();

    protected async override Task<Result<CopernicusPodNews>> FetchNewsInternalAsync(
        CancellationToken ct )
    {
        var doc = new HtmlDocument();

        await using var stream =
            await _httpClient.GetStreamAsync( CopernicusImageDayUrl, ct );
        doc.Load( stream );

        var potdResult = ExtractPotdInfo( doc.DocumentNode );
        if (potdResult.IsFailed) return Result.Fail( potdResult.Errors );

        var potdInfo = potdResult.Value;

        return Result.Ok(
            new CopernicusPodNews() {
                PubDate = potdInfo.PubDate,
                PodDescription = potdInfo
            } );
    }

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        CopernicusPodNews news, CancellationToken ct )
    {
        var imageUrl = news.PodDescription.Url;

        var cachedFilenameResult =
            await DownloadFileAsync( imageUrl, ct );

        if (cachedFilenameResult.IsFailed) {
            return Result.Fail(
                $"Can not download media from {imageUrl}." );
        }

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedFilenameResult.Value,
            Created = news.PubDate,
            Title = news.PodDescription.Title,
            Copyright = $"© {news.PodDescription.Author}",
        };

        return Result.Ok( result );
    }

    public static Result<CopernicusPodDescription> ExtractPotdInfo( HtmlNode documentNode )
    {
        var podItemNode =
            documentNode
            .Descendants( "li" ).FirstOrDefault( x =>
                x.HasClass( "PODItem" ) );

        if (podItemNode is null)
            return Result.Fail( "Can not find pod item node." );

        var imageUrl =
            podItemNode
            .Descendants( "img" ).FirstOrDefault()
            ?.Attributes["src"]
            ?.Value;

        if (string.IsNullOrWhiteSpace( imageUrl ))
            return Result.Fail( "Can not find url of the last image." );

        var title =
            podItemNode
            .Descendants( "div" ).FirstOrDefault( x =>
                x.HasClass( "ItemDescription" ) )
            ?.InnerText ?? string.Empty; // TODO? should we try to get title from img.alt

        var author =
            podItemNode
            .Descendants( "div" ).FirstOrDefault( x =>
                x.HasClass( "ItemPhotographer" ) )
            ?.InnerText ?? string.Empty;

        var rawDate =
            podItemNode
            .Descendants( "span" ).FirstOrDefault( x =>
                x.HasClass( "ItemDate" ) )
            ?.InnerText;

        if (!DateTime.TryParseExact(
                rawDate,
                "dd MMMM yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var pubDate ))
            pubDate = DateTime.Now;

        var potdInfo =
            new CopernicusPodDescription() {
                Author = WebUtility.HtmlDecode( author ),
                Title = WebUtility.HtmlDecode( title ),
                PubDate = pubDate,
                Url = imageUrl
            };

        return Result.Ok( potdInfo );
    }

    private const string CopernicusImageDayUrl = "https://www.copernicus.eu/en/media/image-day";
}
