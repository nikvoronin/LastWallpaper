using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PodLoader<TPodLatestUpdate>(
    IResourceManager resourceManager )
    : IPotdLoader
    where TPodLatestUpdate : PodLatestUpdate
{
    public abstract string Name { get; }

    public async Task<Result<PodUpdateResult>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        Result<PodUpdateResult> result;
        try {
            var latestUpdate = await FetchLatestUpdateInternalAsync( ct );
            if (latestUpdate.IsFailed) return Result.Fail( latestUpdate.Errors );

            if (_resourceManager.PotdExists(
                Name,
                latestUpdate.Value.PubDate ))
                return Result.Fail( "Picture already known." );

            result = await UpdateInternalAsync( latestUpdate.Value, ct );
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
        TPodLatestUpdate latestUpdate,
        CancellationToken ct );
    protected abstract Task<Result<TPodLatestUpdate>> FetchLatestUpdateInternalAsync(
        CancellationToken ct );

    protected readonly IResourceManager _resourceManager = resourceManager;

    private int _interlocked;
}
