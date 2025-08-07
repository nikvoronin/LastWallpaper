using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Apod.Fetchers;

public sealed class ApodWebDescriptionFetcher : IDescriptionFetcher<HtmlPodNews>
{
    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        HtmlPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                PodType = PodType.ApodWeb,
                Url = new( news.Url ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = $"© {news.Author}",
            } ) );
}
