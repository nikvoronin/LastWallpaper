using FluentResults;
using LastWallpaper.Models;
using System;
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

public abstract class PodLoader( HttpClient client ) : IPictureDayLoader
{
    public abstract string Name { get; }

    public async Task<Result<Imago>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        Result<Imago> result;
        try {
            result = await UpdateInternalAsync( ct );
        }
        catch (Exception e)
        when (e is not OperationCanceledException) {
            result = Result.Fail(
                new ExceptionalError(
                    $"Error on updating #{Name} POD", e ) );
        }
        finally {
            Interlocked.Exchange( ref _interlocked, 0 );
        }

        return result;
    }

    protected abstract Task<Result<Imago>> UpdateInternalAsync( CancellationToken ct );

    private int _interlocked;
    protected readonly HttpClient _client = client;
}
