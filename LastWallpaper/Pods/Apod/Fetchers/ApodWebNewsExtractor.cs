using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Linq;
using System.Net;

namespace LastWallpaper.Pods.Apod.Fetchers;

public class ApodWebNewsExtractor(
    Uri baseUri )
    : INewsExtractor<HtmlPodNews>
{
    public Result<HtmlPodNews> ExtractNews( HtmlNode root )
    {
        var centerRootNode =
            root.Descendants( "center" ).FirstOrDefault();

        if (centerRootNode is null)
            return Result.Fail( "Can not find root container 'center'." );

        var apodImageUrl =
            centerRootNode
            .Descendants( "a" ).FirstOrDefault( x =>
                x.GetAttributes( "href" ).Any( IsApodImageHref ) )
            ?.Attributes["href"]
            ?.Value;

        if (apodImageUrl is null)
            return Result.Fail( "Can not find url of the apod image." );

        // TODO: extract publication date
        // TODO: extract title
        // TODO: extract credits (author)

        var potdInfo =
            new HtmlPodNews() {
                PodType = PodType.ApodWeb,
                Author = WebUtility.HtmlDecode( "author" ),
                Title = WebUtility.HtmlDecode( "title" ),
                PubDate = DateTimeOffset.UtcNow,
                Url = $"{_baseUri}/{apodImageUrl}"
            };

        return Result.Ok( potdInfo );

        static bool IsApodImageHref( HtmlAttribute attr ) =>
            attr.Value.StartsWith( $"image/{DateTimeOffset.UtcNow:yyMM}/" )
            && attr.Value.EndsWith( ".jpg" );
    }

    private readonly Uri _baseUri = baseUri;
}
