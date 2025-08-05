using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Nasa.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Nasa.Fetchers;

public sealed class NasaDescriptionFetcher : IDescriptionFetcher<NasaPodNews>
{
    public NasaDescriptionFetcher( HttpClient httpClient )
    {
        ArgumentNullException.ThrowIfNull( httpClient );

        _httpClient = httpClient;
    }

    public async Task<Result<PotdDescription>> FetchDescriptionAsync(
        NasaPodNews news,
        CancellationToken ct )
    {
        var author = "www.nasa.gov";

        if (news.HasCredits) {
            var authorResult = await ExtractAuthorName( news.CreditsPageUrl, ct );
            if (authorResult.IsSuccess)
                author = authorResult.Value;
        }

        return Result.Ok(
            new PotdDescription() {
                PodType = PodType.Nasa,
                Url = news.ImageUrl,
                PubDate = news.PubDate,
                Title = news.Title,
                Description = news.Description,
                Copyright = $"© {author}",
            } );
    }

    private async Task<Result<string>> ExtractAuthorName(
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

    private readonly HttpClient _httpClient;
}
