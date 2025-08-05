using FluentResults;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions.Fetchers;

/// <summary>
/// Defines an interface for asynchronously fetching descriptions related to "Picture of the Day" (PotD) news.
/// </summary>
/// <typeparam name="TPotdNews">
/// The type of news data used to fetch the description, which must implement the <see cref="IPotdNews"/> interface.
/// </typeparam>
/// <remarks>
/// This interface is designed to work with asynchronous operations and supports cancellation via a <see cref="CancellationToken"/>.
/// It returns the fetched description wrapped in a <see cref="Result{T}"/> from the FluentResults library, allowing for robust error handling.
/// </remarks>
public interface IDescriptionFetcher<TPotdNews>
    where TPotdNews : IPotdNews
{
    /// <summary>
    /// Asynchronously fetches a description for the provided PotD news.
    /// </summary>
    /// <param name="news">
    /// The PotD news data used to fetch the description.
    /// </param>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> allows you to signal to a task or operation that it should stop its work and terminate gracefully.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, which returns a <see cref="Result{T}"/> containing the fetched <see cref="PotdDescription"/>.
    /// If the operation fails, the result will indicate the error.
    /// </returns>
    Task<Result<PotdDescription>> FetchDescriptionAsync(
        TPotdNews news,
        CancellationToken ct );
}
