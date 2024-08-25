using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class HtmlPodLoader<TPodNews>(
    HttpClient httpClient,
    Uri podNewsPage,
    IResourceManager resourceManager )
    : HttpPodLoader<TPodNews>( httpClient, resourceManager )
    where TPodNews : PodNews
{
    protected abstract Result<TPodNews> ExtractHtmlDescription( HtmlNode rootNode );

    protected async override Task<Result<TPodNews>> FetchNewsInternalAsync(
        CancellationToken ct )
    {
        await using var stream =
            await _httpClient.GetStreamAsync( _podNewsPage, ct );
        _doc.Load( stream );

        var podNewsResult = ExtractHtmlDescription( _doc.DocumentNode );
        if (podNewsResult.IsFailed) return Result.Fail( podNewsResult.Errors );

        var iotdInfo = podNewsResult.Value;

        return Result.Ok( podNewsResult.Value );
    }

    protected readonly HtmlDocument _doc = new();
    protected readonly Uri _podNewsPage = podNewsPage;
}
