using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Fetchers;

public sealed class HtmlDescriptionFetcher : IDescriptionFetcher<HtmlPodNews>
{
    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        HtmlPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                PodType = news.PodType,
                Url = new( news.Url ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = $"© {news.Author}",
            } ) );
}
