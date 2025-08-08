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
                $"The number {centerNodes.Count} of 'center' containers does not match. " );
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

        var dateTimeRaw =
            centerNodes.FirstOrDefault()
            ?.Descendants( "p" ).Skip( 1 ).FirstOrDefault()
            ?.FirstChild
            ?.InnerText
            ?.Trim();
        #endregion

        #region Publication date
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
            ?.InnerText
            ?.Trim();
        #endregion

        // TODO: extract credits (author)

        var potdInfo =
            new HtmlPodNews() {
                PodType = PodType.ApodWeb,
                Author = WebUtility.HtmlDecode( "author" ),
                Title = WebUtility.HtmlDecode( "title" ),
                PubDate = publicationDate,
                Url = $"{_baseUri}/{apodImageUrl}"
            };

        return Result.Ok( potdInfo );

        static bool IsApodImageHref( HtmlAttribute attr ) =>
            attr.Value.StartsWith( $"image/{DateTimeOffset.UtcNow:yyMM}/" )
            && attr.Value.EndsWith( ".jpg" );
    }

    private readonly Uri _baseUri = baseUri;
}
