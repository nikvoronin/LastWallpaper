using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Astrobin.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Astrobin;

public sealed class AstrobinPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HttpPodLoader( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Astrobin ).ToLower();

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        CancellationToken ct )
    {
        await using var stream =
            await _httpClient.GetStreamAsync( AstrobinIotdArchiveUrl, ct );
        _doc.Load( stream );

        var iotdResult = ExtractIotdInfo( _doc.DocumentNode );
        if (iotdResult.IsFailed) return Result.Fail( iotdResult.Errors );

        var iotdInfo = iotdResult.Value;

        return Result.Fail( "Not implemented yet." );
    }

    public static Result<AbinIotdDescription> ExtractIotdInfo( HtmlNode docNode )
    {
        var hdPageKeySegment =
            docNode
                .Descendants( "figure" ).FirstOrDefault()
                ?.Descendants( "a" ).FirstOrDefault()
                ?.ChildAttributes( "href" ).FirstOrDefault()
                ?.Value;

        if (hdPageKeySegment is null)
            return Result.Fail( "Can not find the last image." );

        var descriptionNode =
            docNode
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

        if (!DateTimeOffset.TryParseExact(
            pubDateRaw,
            "MM/dd/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTimeOffset pubDate ))
            pubDate = DateTimeOffset.Now;

        var iotdInfo =
            new AbinIotdDescription() {
                Author = author,
                Title = title,
                PubDate = pubDate,

                HdPageUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    HdImagePageUrlFormat,
                    hdPageKeySegment )
            };

        return Result.Ok( iotdInfo );
    }

    private static readonly HtmlDocument _doc = new();

    private static readonly CompositeFormat HdImagePageUrlFormat =
        CompositeFormat.Parse( AstrobinBaseUrl + "/full{0}0/" );

    private const string AstrobinIotdArchiveUrl = AstrobinBaseUrl + "/iotd/archive/";
    private const string AstrobinBaseUrl = "https://www.astrobin.com";
}
