using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PodLoader : IPotdLoader
{
    public abstract string Name { get; }
    protected abstract Task<Result<Imago>> UpdateInternalAsync( CancellationToken ct );

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
                    $"Error while updating #{Name} POD", e ) );
        }
        finally {
            Interlocked.Exchange( ref _interlocked, 0 );
        }

        return result;
    }

    private int _interlocked;
}
