using FluentResults;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions.Fetchers;

/// <summary>
/// Defines an interface for asynchronously fetching "Picture of the Day" (PotD) updates based on provided news data.
/// </summary>
/// <typeparam name="TPotdNews">
/// The type of news data used to fetch the PotD update, which must implement the <see cref="IPotdNews"/> interface.
/// </typeparam>
/// <remarks>
/// This interface is designed to work with asynchronous operations and supports cancellation via a <see cref="CancellationToken"/>.
/// It returns the fetched PotD update wrapped in a <see cref="Result{T}"/> from the FluentResults library, enabling robust error handling and result management.
/// </remarks>
public interface IPotdFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    /// <summary>
    /// Asynchronously fetches a "Picture of the Day" (PotD) update based on the provided news data.
    /// </summary>
    /// <param name="news">
    /// The PotD news data used to fetch the update.
    /// </param>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> allows you to signal to a task or operation that it should stop its work and terminate gracefully.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, which returns a <see cref="Result{T}"/> containing the fetched <see cref="PodUpdateResult"/>.
    /// If the operation fails, the result will indicate the error.
    /// </returns>
    Task<Result<PodUpdateResult>> FetchPotdAsync(
        TPotdNews news,
        CancellationToken ct );
}
