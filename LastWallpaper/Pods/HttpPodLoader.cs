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
    /// <summary>
    /// Download remote resource to local temporary file.
    /// </summary>
    /// <param name="url">Source url to download from.</param>
    /// <param name="ct">Token to cancel the operation.</param>
    /// <returns>Filename of a temporary file.</returns>
    public async Task<Result<string>> DownloadFileAsync(
        string url, CancellationToken ct )
    {
        await using var imageStream =
            await _httpClient.GetStreamAsync( url, ct );

        await using var fileStream =
            _resourceManager.CreateTemporaryFileStream();
        var cachedImageFilename = fileStream.Name;

        await imageStream.CopyToAsync( fileStream, ct );

        return Result.Ok( cachedImageFilename );
    }

    protected readonly HttpClient _httpClient = httpClient;
}
