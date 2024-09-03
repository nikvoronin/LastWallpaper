using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Nasa.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Nasa;

public class NasaPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HtmlPodLoader<NasaPodNews>(
        httpClient,
        new( NasaImageDayUrl ),
        resourceManager )
{
    public override string Name => nameof( PodType.Nasa ).ToLower();

    protected override Result<NasaPodNews> FindNews( HtmlNode rootNode )
    {
        DateTime pubDate = DateTime.Now;

        var galleryItemNode =
            rootNode
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
                PubDate = pubDate,
                Title = title,
                CreditsPageUrl = creditsPageUrl,
                ImageUrl = new( imageUrl ),
                Description = sceneDescription
            } );
    }

    protected async override Task<Result<PotdDescription>> GetDescriptionAsync(
        NasaPodNews news, CancellationToken ct )
    {
        var author = "www.nasa.gov";

        if (news.HasCredits) {
            var authorResult = await ExtractAuthorName( news.CreditsPageUrl, ct );
            if (authorResult.IsSuccess)
                author = authorResult.Value;
        }

        return Result.Ok(
            new PotdDescription() {
                Url = news.ImageUrl,
                PubDate = news.PubDate,
                Title = news.Title,
                Description = news.Description,
                Copyright = $"© {author}",
            } );
    }

    protected async Task<Result<string>> ExtractAuthorName(
        string? creditsPageUrl,
        CancellationToken ct )
    {
        var doc = new HtmlDocument();
        await using var streamDoc =
            await _httpClient.GetStreamAsync( creditsPageUrl, ct );
        doc.Load( streamDoc );

        var creditParts =
            doc.DocumentNode
            .Descendants( "div" )
            .FirstOrDefault( x =>
                x.HasClass( "hds-attachment-single__credit" ) )
            ?.GetDirectInnerText()
            ?.Trim()
            ?.Split( ':' );

        return
            creditParts?.Length != 2 ? Result.Fail( "Can not find author name." )
            : Result.Ok( creditParts[1].Trim() ); // Image Credit: NASA/John C. Do;
    }

    private const string NasaBaseUrl = "https://www.nasa.gov";
    private const string NasaImageDayUrl = NasaBaseUrl + "/image-of-the-day/";
}
