using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace LastWallpaper.Pods.Copernicus.Fetchers;

public class CopernicusNewsExtractor(
    Uri copernicusSystemFilesUri )
    : INewsExtractor<HtmlPodNews>
{
    public Result<HtmlPodNews> ExtractNews( HtmlNode root )
    {
        var articleNode =
            root
            .Descendants( "article" ).FirstOrDefault();

        if (articleNode is null) return Result.Fail( "Can not find pod item node." );

        var imageNode =
            articleNode
            .Descendants( "img" ).FirstOrDefault();

        var imageUrlPartsRaw =
            imageNode
            ?.Attributes["src"]
            ?.Value;

        if (imageUrlPartsRaw is null) return Result.Fail( "Can not find url of the last image." );

        var urlParts = new Uri( _copernicusSystemFilesUri, imageUrlPartsRaw );

        var imageUrl =
            _copernicusSystemFilesUri
            + "/" + string.Concat( urlParts.Segments[^3..] );

        var tags =
            string.Join( " ∙ ",
                articleNode
                .Descendants( "div" )
                .Where( x =>
                    x.HasClass( "search-tag-btn" )
                    && !string.IsNullOrWhiteSpace( x.InnerText ) )
                .Select( n =>
                    n.InnerText
                    .Trim()
                    .ReplaceLineEndings() )
                .ToList()
            );

        tags =
            string.IsNullOrWhiteSpace( tags ) ? string.Empty
            : Environment.NewLine + tags;

        var title =
            (
                imageNode
                ?.Attributes["alt"]
                ?.Value
                ?? string.Empty // TODO? should we try to get title from H4 element
            ) + tags;

        var dateTimeRaw =
            articleNode
            .Descendants( "time" ).FirstOrDefault()
            ?.Attributes["datetime"]
            ?.Value;

        if (!DateTime.TryParse(
                dateTimeRaw,
                CultureInfo.InvariantCulture,
                out var pubDate ))
            pubDate = DateTime.Now;

        var podNews =
            new HtmlPodNews() {
                PodType = PodType.Copernicus,
                Author = "© www.copernicus.eu",
                Title = WebUtility.HtmlDecode( title ),
                PubDate = pubDate,
                Url = imageUrl
            };

        return Result.Ok( podNews );
    }

    private readonly Uri _copernicusSystemFilesUri = copernicusSystemFilesUri;
}
