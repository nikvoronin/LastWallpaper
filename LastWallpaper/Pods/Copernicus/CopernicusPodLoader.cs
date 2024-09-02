using FluentResults;
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

namespace LastWallpaper.Pods.Copernicus;

public sealed class CopernicusPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HtmlPodLoader<HtmlPodNews>(
        httpClient,
        new( CopernicusImageDayUrl ),
        resourceManager )
{
    public override string Name => nameof( PodType.Copernicus ).ToLower();

    protected override Result<HtmlPodNews> FindNews( HtmlNode rootNode )
    {
        var articleNode =
            rootNode
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

        var urlParts = new Uri( CopernicusBaseUrl + imageUrlPartsRaw );

        var imageUrl =
            CopernicusSystemFilesUrl + "/"
            + string.Concat( urlParts.Segments[^3..] );

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
                Author = "© www.copernicus.eu",
                Title = WebUtility.HtmlDecode( title ),
                PubDate = pubDate,
                Url = imageUrl
            };

        return Result.Ok( podNews );
    }

    protected override Task<Result<PotdDescription>> GetDescriptionAsync(
        HtmlPodNews news, CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                Url = new( news.Url ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = news.Author,
            } ) );

    private const string CopernicusBaseUrl = "https://www.copernicus.eu";
    private const string CopernicusImageDayUrl = CopernicusBaseUrl + "/en/media/image-day";
    private const string CopernicusSystemFilesUrl = CopernicusBaseUrl + "/system/files";
}
