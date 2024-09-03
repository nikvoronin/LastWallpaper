using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class HttpPodLoader<TPodNews>(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : PodLoader<TPodNews>( resourceManager )
    where TPodNews : PodNews
{
    protected abstract Task<Result<PotdDescription>> GetDescriptionAsync(
        TPodNews news, CancellationToken ct );

    protected async override Task<Result<PodUpdateResult>> UpdateInternalAsync(
        TPodNews news,
        CancellationToken ct )
    {
        var potdResults = await GetDescriptionAsync( news, ct );

        if (potdResults.IsFailed) return Result.Fail( potdResults.Errors );

        var potd = potdResults.Value;

        await using var imageStream =
            await _httpClient.GetStreamAsync( potd.Url, ct );

        await using var fileStream =
            _resourceManager.CreateTemporaryFileStream();
        var cachedImageFilename = fileStream.Name;

        await imageStream.CopyToAsync( fileStream, ct );

        var result = new PodUpdateResult() {
            PodName = Name,
            Filename = cachedImageFilename,
            Created = news.PubDate,
            Title = potd.Title,
            Description = potd.Description,
            Copyright = potd.Copyright,
        };

        return Result.Ok( result );
    }

    protected readonly HttpClient _httpClient = httpClient;
}
