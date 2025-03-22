using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Bing.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Bing.Fetchers;

public sealed class BingDescriptionFetcher : IDescriptionFetcher<BingPodNews>
{
    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        BingPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                PodType = news.PodType,
                Url = new( news.LastImageUrl ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = news.Copyright
            } ) );
}
