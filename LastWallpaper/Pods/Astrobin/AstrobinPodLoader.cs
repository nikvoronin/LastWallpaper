using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Astrobin;

public class AstrobinPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HtmlPodLoader<HtmlPodNews>(
        httpClient,
        new( AstrobinIotdArchiveUrl ),
        resourceManager )
{
    public override string Name => nameof( PodType.Astrobin ).ToLower();

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        HtmlPodNews news, CancellationToken ct )
    {
        var doc = new HtmlDocument();
        await using var streamHd =
            await _httpClient.GetStreamAsync( news.Url, ct );
        doc.Load( streamHd );

        var hdImageResult = ExtractHdImageUrl( doc.DocumentNode );
        if (hdImageResult.IsFailed) return Result.Fail( hdImageResult.Errors );

        var hdImageUrl = hdImageResult.Value;

        var cachedFilenameResult =
            await DownloadFileAsync( hdImageUrl, ct );

        if (cachedFilenameResult.IsFailed) {
            return Result.Fail(
                $"Can not download media from {hdImageUrl}." );
        }

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedFilenameResult.Value,
            Created = news.PubDate,
            Title = news.Title,
            Copyright = $"© {news.Author}",
        };

        return Result.Ok( result );
    }

    protected Result<string> ExtractHdImageUrl( HtmlNode documentNode )
    {
        var hdImageUrl =
            documentNode
                .Descendants( "figure" ).FirstOrDefault()
                ?.Descendants( "img" ).FirstOrDefault()
                ?.ChildAttributes( "src" ).FirstOrDefault()
                ?.Value;

        return
            hdImageUrl is null ? Result.Fail( "Can not find high resolution image." )
            : Result.Ok( hdImageUrl );
    }

    protected override Result<HtmlPodNews> ExtractHtmlDescription( HtmlNode documentNode )
    {
        var hdPageKeySegment =
            documentNode
                .Descendants( "figure" ).FirstOrDefault()
                ?.Descendants( "a" ).FirstOrDefault()
                ?.ChildAttributes( "href" ).FirstOrDefault()
                ?.Value;

        if (hdPageKeySegment is null)
            return Result.Fail( "Can not find the last image." );

        var descriptionNode =
            documentNode
                .Descendants( "div" )
                .FirstOrDefault( x =>
                    x.HasClass( "data" )
                    && x.HasClass( "hidden-phone" ) );

        if (descriptionNode is null)
            return Result.Fail( "Can not find description of the last image." );

        var title =
            descriptionNode
            .Descendants( "h3" ).FirstOrDefault()
            ?.InnerText ?? string.Empty; // TODO? should we try to get from hdPageKeySegment

        var author =
            (
                descriptionNode
                .Descendants( "a" ).FirstOrDefault( x =>
                    x.HasClass( "astrobin-username" ) )
                ?.InnerText ?? string.Empty // TODO? should we try to get from hdPageKeySegment
            )
            .Replace( "\n", string.Empty )
            .Replace( "\r", string.Empty )
            .Trim();

        var pubDateRaw =
            (
                descriptionNode
                .Descendants( "p" ).FirstOrDefault()
                ?.InnerText ?? string.Empty
            )
            .Replace( "\n", string.Empty )
            .Replace( "\r", string.Empty )
            .Trim()
            .Split( ',' )
            .FirstOrDefault();

        if (!DateTime.TryParseExact(
            pubDateRaw,
            "MM/dd/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime pubDate ))
            pubDate = DateTime.Now;

        var hdPageUrlFormat =
            hdPageKeySegment
            .Count( x => x == '/' ) > 2
                ? HdImagePageUrlFormat
                : HdImagePageUrlFormatZero;

        return Result.Ok(
            new HtmlPodNews() {
                PubDate = pubDate,
                Title = WebUtility.HtmlDecode( title ),
                Author = WebUtility.HtmlDecode( author ),
                Url = string.Format(
                    CultureInfo.InvariantCulture,
                    hdPageUrlFormat,
                    hdPageKeySegment )
            } );

    }

    private static readonly CompositeFormat HdImagePageUrlFormatZero =
        CompositeFormat.Parse( AstrobinBaseUrl + "/full{0}0/" );

    private static readonly CompositeFormat HdImagePageUrlFormat =
        CompositeFormat.Parse( AstrobinBaseUrl + "/full{0}" );

    private const string AstrobinIotdArchiveUrl = AstrobinBaseUrl + "/iotd/archive/";
    private const string AstrobinBaseUrl = "https://www.astrobin.com";
}
