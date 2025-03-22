using FluentResults;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;
namespace LastWallpaper.Abstractions.Fetchers;

public interface IDescriptionFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    Task<Result<PotdDescription>> FetchDescriptionAsync(
        TPotdNews news,
        CancellationToken ct );
}
