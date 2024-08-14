using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PodLoader<TPodNews>(
    IResourceManager resourceManager )
    : IPotdLoader
    where TPodNews : PodNews
{
    public abstract string Name { get; }

    public async Task<Result<PodUpdateResult>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        Result<PodUpdateResult> result;
        try {
            var newsResult = await FetchNewsInternalAsync( ct );
            if (newsResult.IsFailed) return Result.Fail( newsResult.Errors );

            if (_resourceManager.PotdExists(
                Name,
                newsResult.Value.PubDate ))
                return Result.Fail( "Picture already known." );

            result = await UpdateInternalAsync( newsResult.Value, ct );
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

    protected abstract Task<Result<PodUpdateResult>> UpdateInternalAsync(
        TPodNews news, CancellationToken ct );
    protected abstract Task<Result<TPodNews>> FetchNewsInternalAsync(
        CancellationToken ct );

    protected readonly IResourceManager _resourceManager = resourceManager;

    private int _interlocked;
}
