using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace LastWallpaper.Pods.Astrobin.Fetchers;

public class AstrobinNewsExtractor : INewsExtractor<HtmlPodNews>
{
    public AstrobinNewsExtractor( Uri astrobinBaseUri )
    {
        ArgumentNullException.ThrowIfNull( astrobinBaseUri );

        _astrobinBaseUri = astrobinBaseUri;

        _hdImagePageUrlFormatZero =
            CompositeFormat.Parse( _astrobinBaseUri + "/full{0}0/" );
        _hdImagePageUrlFormat =
            CompositeFormat.Parse( _astrobinBaseUri + "/full{0}" );
    }

    public Result<HtmlPodNews> ExtractNews( HtmlNode root )
    {
        var hdPageKeySegment =
            root
                .Descendants( "figure" ).FirstOrDefault()
                ?.Descendants( "a" ).FirstOrDefault()
                ?.ChildAttributes( "href" ).FirstOrDefault()
                ?.Value;

        if (hdPageKeySegment is null)
            return Result.Fail( "Can not find the last image." );

        var descriptionNode =
            root
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
                ? _hdImagePageUrlFormat
                : _hdImagePageUrlFormatZero;

        return Result.Ok(
            new HtmlPodNews() {
                PodType = PodType.Astrobin,
                PubDate = pubDate,
                Title = WebUtility.HtmlDecode( title ),
                Author = WebUtility.HtmlDecode( author ),
                Url = string.Format(
                    CultureInfo.InvariantCulture,
                    hdPageUrlFormat,
                    hdPageKeySegment )
            } );
    }

    private readonly Uri _astrobinBaseUri;
    private readonly CompositeFormat _hdImagePageUrlFormatZero;
    private readonly CompositeFormat _hdImagePageUrlFormat;
}
