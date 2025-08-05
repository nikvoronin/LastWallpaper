using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Fetchers;

public class HttpPotdFetcher<TPotdNews> : IPotdFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    public HttpPotdFetcher(
        HttpClient httpClient,
        IDescriptionFetcher<TPotdNews> descriptionFetcher,
        IResourceManager resourceManager )
    {
        ArgumentNullException.ThrowIfNull( httpClient );
        ArgumentNullException.ThrowIfNull( descriptionFetcher );
        ArgumentNullException.ThrowIfNull( resourceManager );

        _httpClient = httpClient;
        _descriptionFetcher = descriptionFetcher;
        _resourceManager = resourceManager;
    }

    public async Task<Result<PodUpdateResult>> FetchPotdAsync(
        TPotdNews news,
        CancellationToken ct )
    {
        var potdResults = await _descriptionFetcher.FetchDescriptionAsync( news, ct );

        if (potdResults.IsFailed) return Result.Fail( potdResults.Errors );

        var potd = potdResults.Value;

        await using var imageStream =
            await _httpClient.GetStreamAsync( potd.Url, ct );

        await using var fileStream =
            _resourceManager.CreateTemporaryFileStream();
        var cachedImageFilename = fileStream.Name;

        await imageStream.CopyToAsync( fileStream, ct );

        var result = new PodUpdateResult() {
            PodType = news.PodType,
            Filename = cachedImageFilename,
            Created = news.PubDate,
            Title = potd.Title,
            Description = potd.Description,
            Copyright = potd.Copyright,
        };

        return Result.Ok( result );
    }

    private readonly HttpClient _httpClient;
    private readonly IResourceManager _resourceManager;
    private readonly IDescriptionFetcher<TPotdNews> _descriptionFetcher;
}
