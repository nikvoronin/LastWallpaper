using FluentResults;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions.Fetchers;

/// <summary>
/// Defines an interface for asynchronously fetching "Picture of the Day" (PotD) news.
/// </summary>
/// <typeparam name="TPotdNews">The type of news data to fetch, which must implement the <see cref="IPotdNews"/> interface.</typeparam>
/// <remarks>
/// This interface is designed to work with asynchronous operations and supports cancellation via a <see cref="CancellationToken"/>.
/// It returns the fetched news data wrapped in a <see cref="Result{T}"/> from the FluentResults library, enabling robust error handling.
/// </remarks>
public interface INewsFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    /// <summary>
    /// Asynchronously fetches PotD news data.
    /// </summary>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> allows you to signal to a task or operation that it should stop its work and terminate gracefully.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, which returns a <see cref="Result{T}"/> containing the fetched news data of type <typeparamref name="TPotdNews"/>.
    /// If the operation fails, the result will indicate the error.
    /// </returns>
    Task<Result<TPotdNews>> FetchNewsAsync(
        CancellationToken ct );
}
