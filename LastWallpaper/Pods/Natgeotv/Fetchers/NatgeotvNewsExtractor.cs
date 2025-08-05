using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace LastWallpaper.Pods.Natgeotv.Fetchers;

public class NatgeotvNewsExtractor : INewsExtractor<HtmlPodNews>
{
    public Result<HtmlPodNews> ExtractNews( HtmlNode root )
    {
        var podItemNode =
            root
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
            new HtmlPodNews() {
                PodType = PodType.Natgeotv,
                Author = WebUtility.HtmlDecode( author ),
                Title = WebUtility.HtmlDecode( title ),
                PubDate = pubDate,
                Url = imageUrl
            };

        return Result.Ok( potdInfo );
    }
}
