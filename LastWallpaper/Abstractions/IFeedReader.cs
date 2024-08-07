using FluentResults;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions;

public interface IFeedReader<T>
{
    public Task<Result<T>> ParseFeedAsync(
        string url,
        HttpClient httpClient,
        CancellationToken ct );
}