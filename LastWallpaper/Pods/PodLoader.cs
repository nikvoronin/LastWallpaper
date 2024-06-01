using FluentResults;
using LastWallpaper.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public interface IPictureDayLoader
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
    Task<Result<Imago>> UpdateAsync( CancellationToken ct );
}

public abstract class PodLoader : IPictureDayLoader
{
    public abstract string Name { get; }

    protected PodLoader( HttpClient client )
    {
        _client = client;
    }

    public async Task<Result<Imago>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        var result = await UpdateInternalAsync( ct );

        Interlocked.Exchange( ref _interlocked, 0 );
        return result;
    }

    protected abstract Task<Result<Imago>> UpdateInternalAsync( CancellationToken ct );

    private int _interlocked;
    protected readonly HttpClient _client;
}
