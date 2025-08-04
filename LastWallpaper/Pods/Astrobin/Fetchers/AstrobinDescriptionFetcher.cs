using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Astrobin.Fetchers;

public sealed class AstrobinDescriptionFetcher : IDescriptionFetcher<HtmlPodNews>
{
    public AstrobinDescriptionFetcher( HttpClient httpClient )
    {
        ArgumentNullException.ThrowIfNull( httpClient );

        _httpClient = httpClient;
    }

    public async Task<Result<PotdDescription>> FetchDescriptionAsync(
        HtmlPodNews news,
        CancellationToken ct )
    {
        var doc = new HtmlDocument();
        await using var streamHd =
            await _httpClient.GetStreamAsync( news.Url, ct );
        doc.Load( streamHd );

        var hdImageResult = ExtractHdImageUrl( doc.DocumentNode );
        if (hdImageResult.IsFailed) return Result.Fail( hdImageResult.Errors );

        var hdImageUrl = new Uri( hdImageResult.Value );
        var fileExtension = new FileInfo( hdImageUrl.Segments[^1] ).Extension;
        if (fileExtension != ".jpeg" && fileExtension != ".jpg") {
            return Result.Fail(
                $"Wrong media format '{fileExtension}' of the image file." );
        }

        return Result.Ok(
            new PotdDescription() {
                PodType = PodType.Astrobin,
                Url = hdImageUrl,
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = $"© {news.Author}",
            } );
    }

    public static Result<string> ExtractHdImageUrl( HtmlNode documentNode )
    {
        var hdImageUrl =
            documentNode
                .Descendants( "figure" ).FirstOrDefault()
                ?.Descendants( "img" ).FirstOrDefault()
                ?.ChildAttributes( "src" ).FirstOrDefault()
                ?.Value;

        return
            hdImageUrl is null ? Result.Fail( "Can not find high resolution image." )
            : Result.Ok( hdImageUrl );
    }

    private readonly HttpClient _httpClient;
}
