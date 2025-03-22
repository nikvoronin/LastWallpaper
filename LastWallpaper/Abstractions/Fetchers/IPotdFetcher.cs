using FluentResults;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;
namespace LastWallpaper.Abstractions.Fetchers;

public interface IPotdFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    Task<Result<PodUpdateResult>> FetchPotdAsync(
        TPotdNews news,
        CancellationToken ct );
}
