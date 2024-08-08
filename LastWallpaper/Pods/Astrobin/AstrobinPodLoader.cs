using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Astrobin;

public sealed class AstrobinPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : HttpPodLoader( httpClient, resourceManager )
{
    public override string Name => nameof( PodType.Astrobin ).ToLower();

    protected override async Task<Result<PodUpdateResult>> UpdateInternalAsync(
        CancellationToken ct )
    {
        return Result.Fail( "Not implemented yet." );
    }
}
