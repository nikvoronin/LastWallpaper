using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Nasa.Models;
using System;
using System.Linq;

namespace LastWallpaper.Pods.Nasa.Fetchers;

public class NasaNewsExtractor : INewsExtractor<NasaPodNews>
{
    public Result<NasaPodNews> ExtractNews( HtmlNode root )
    {
        DateTime pubDate = DateTime.Now;

        var galleryItemNode =
            root
            .Descendants( "a" )
            .FirstOrDefault( x =>
                x.HasClass( "hds-gallery-item-link" )
                && x.HasClass( "hds-gallery-image-link" ) );

        if (galleryItemNode is null) return Result.Fail( "Can not find the latest gallery item node." );

        var title =
            galleryItemNode
            .Descendants( "div" )
            .FirstOrDefault( x =>
                x.HasClass( "hds-gallery-item-caption" )
                && x.HasClass( "hds-gallery-image-caption" ) )
            ?.GetDirectInnerText()
            ?.Trim()
            ?? string.Empty;

        var imageNode =
            galleryItemNode
            .Descendants( "img" ).FirstOrDefault();

        if (imageNode is null) return Result.Fail( "Can not find img element with image url." );

        var imageUrl =
            imageNode
            ?.ChildAttributes( "src" ).FirstOrDefault()
            ?.Value;

        if (imageUrl is null) return Result.Fail( "Can not find url of the image of the day." );

        var sceneDescription =
            imageNode
            ?.ChildAttributes( "alt" ).FirstOrDefault()
            ?.Value;

        var creditsPageUrl =
            galleryItemNode
            .ChildAttributes( "href" ).FirstOrDefault()
            ?.Value
            ?.Trim();

        return Result.Ok(
            new NasaPodNews() {
                PodType = PodType.Nasa,
                PubDate = pubDate,
                Title = title,
                CreditsPageUrl = creditsPageUrl,
                ImageUrl = new( imageUrl ),
                Description = sceneDescription
            } );
    }
}
