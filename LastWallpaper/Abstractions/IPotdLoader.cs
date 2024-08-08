using FluentResults;
using LastWallpaper.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions;

public interface IPotdLoader
{
    /// <summary>
    /// Name or prefix of the POD loader.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Updates and downloads pictures of the day.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Collection of pathes to downloaded images or empty collection otherwise.
    /// </returns>
    Task<Result<PodUpdateResult>> UpdateAsync( CancellationToken ct );
}
