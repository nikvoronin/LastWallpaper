﻿using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Natgeotv;

public class NatgeotvPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HtmlPodLoader<HtmlPodNews>(
        httpClient,
        new( NatgeotvCaPotdUrl ),
        resourceManager )
{
    public override string Name => nameof( PodType.Natgeotv ).ToLower();

    protected override Task<Result<PotdDescription>> GetDescriptionAsync(
        HtmlPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                Url = new( news.Url ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = $"© {news.Author}",
            } ) );

    protected override Result<HtmlPodNews> FindNews( HtmlNode rootNode )
    {
        var podItemNode =
            rootNode
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
                Author = WebUtility.HtmlDecode( author ),
                Title = WebUtility.HtmlDecode( title ),
                PubDate = pubDate,
                Url = imageUrl
            };

        return Result.Ok( potdInfo );
    }

    private const string NatgeotvCaPotdUrl = "https://www.natgeotv.com/ca/photo-of-the-day";
}
