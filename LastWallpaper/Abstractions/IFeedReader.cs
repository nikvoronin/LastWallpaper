using FluentResults;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions;

public interface IFeedReader<T>
{
    public Task<Result<T>> ParseFeedAsync(
        Uri uri,
        HttpClient httpClient,
        CancellationToken ct );
}