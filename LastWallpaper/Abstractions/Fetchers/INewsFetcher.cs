using FluentResults;
using System.Threading;
using System.Threading.Tasks;
namespace LastWallpaper.Abstractions.Fetchers;

public interface INewsFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    Task<Result<TPotdNews>> FetchNewsAsync(
        CancellationToken ct );
}
