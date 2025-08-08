using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace LastWallpaper.Pods.Apod.Fetchers;

public class ApodWebNewsExtractor(
    Uri baseUri )
    : INewsExtractor<HtmlPodNews>
{
    public Result<HtmlPodNews> ExtractNews( HtmlNode root )
    {
        var centerNodes =
            root.Descendants( "center" )
            .ToList();

        if (centerNodes.Count < 2) {
            return Result.Fail(
                $"The number of 'center' containers '{centerNodes.Count}' does not match. " );
        }

        #region Apod Image URL
        var apodImageUrl =
            centerNodes[0]
            ?.Descendants( "a" ).FirstOrDefault( x =>
                x.GetAttributes( "href" ).Any( IsApodImageHref ) )
            ?.Attributes["href"]
            ?.Value;

        if (apodImageUrl is null)
            return Result.Fail( "Can not find url of the apod image." );
        #endregion

        #region Publication date
        var dateTimeRaw =
            centerNodes.FirstOrDefault()
            ?.Descendants( "p" ).Skip( 1 ).FirstOrDefault()
            ?.FirstChild
            ?.InnerText
            ?.Trim();

        if (!DateTimeOffset.TryParseExact(
                dateTimeRaw,
                "yyyy MMMM d",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTimeOffset publicationDate ))
            publicationDate = DateTimeOffset.Now;
        #endregion

        #region Title and/or description
        var title =
            centerNodes[1]
            ?.Descendants( "b" ).FirstOrDefault()
            ?.InnerHtml
            ?.Trim()
            ?? string.Empty;
        #endregion

        #region Credits and author(s)
        var credits =
            centerNodes[1]
            ?.InnerText
            ?.Split( ':' )[^1]
            .Trim()
            .Replace( "\r", " " ).Replace( "\n", " " )
            .Replace( "  ", " " )
            ?? string.Empty;
        #endregion

        var potdInfo =
            new HtmlPodNews() {
                PodType = PodType.ApodWeb,
                Author = WebUtility.HtmlDecode( credits ),
                Title = WebUtility.HtmlDecode( title ),
                PubDate = publicationDate,
                Url = $"{_baseUri}/{apodImageUrl}"
            };

        return Result.Ok( potdInfo );

        static bool IsApodImageHref( HtmlAttribute attr ) =>
            attr.Value.StartsWith( $"image/{DateTimeOffset.UtcNow:yyMM}/" )
            && (
                attr.Value.EndsWith( ".jpg" )
                || attr.Value.EndsWith( ".png" )
                || attr.Value.EndsWith( ".jpeg" )
            );
    }

    private readonly Uri _baseUri = baseUri;
}
