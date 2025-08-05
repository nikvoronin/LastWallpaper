using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Fetchers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Fetchers;

public class HtmlNewsFetcher<TPotdNews> : INewsFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    public HtmlNewsFetcher(
        HttpClient httpClient,
        Uri podNewsPage,
        INewsExtractor<TPotdNews> newsExtractor )
    {
        ArgumentNullException.ThrowIfNull( httpClient );
        ArgumentNullException.ThrowIfNull( podNewsPage );
        ArgumentNullException.ThrowIfNull( newsExtractor );

        _httpClient = httpClient;
        _podNewsPage = podNewsPage;
        _newsExtractor = newsExtractor;
    }

    public async Task<Result<TPotdNews>> FetchNewsAsync( CancellationToken ct )
    {
        await using var stream =
            await _httpClient.GetStreamAsync( _podNewsPage, ct );
        _doc.Load( stream );

        var podNewsResult = _newsExtractor.ExtractNews( _doc.DocumentNode );
        if (podNewsResult.IsFailed) return Result.Fail( podNewsResult.Errors );

        return Result.Ok( podNewsResult.Value );
    }

    private readonly HtmlDocument _doc = new();

    private readonly Uri _podNewsPage;
    private readonly HttpClient _httpClient;
    private readonly INewsExtractor<TPotdNews> _newsExtractor;
}
