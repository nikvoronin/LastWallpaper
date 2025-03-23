using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic;

public class PodLoader<TPotdNews> : IPotdLoader
    where TPotdNews : IPotdNews
{
    public PodType PodType { get; }

    public PodLoader(
        PodType podType,
        INewsFetcher<TPotdNews> newsFetcher,
        IPotdFetcher<TPotdNews> potdFetcher,
        IResourceManager resourceManager )
    {
        if (!Enum.IsDefined( podType )) {
            throw new InvalidEnumArgumentException(
                $"Unknown pod type {podType}.",
                (int)podType,
                typeof( PodType ) );
        }

        ArgumentNullException.ThrowIfNull( newsFetcher );
        ArgumentNullException.ThrowIfNull( potdFetcher );
        ArgumentNullException.ThrowIfNull( resourceManager );

        PodType = podType;
        _newsFetcher = newsFetcher;
        _potdFetcher = potdFetcher;
        _resourceManager = resourceManager;
    }

    public async Task<Result<PodUpdateResult>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        Result<PodUpdateResult> result;
        try {
            var newsResult = await _newsFetcher.FetchNewsAsync( ct );
            if (newsResult.IsFailed) return Result.Fail( newsResult.Errors );

            if (_resourceManager.PotdExists(
                    PodType,
                    newsResult.Value.PubDate ))
                return Result.Fail( "Picture already known." );

            result =
                await _potdFetcher.FetchPotdAsync(
                    newsResult.Value,
                    ct );
        }
        catch (Exception e)
        when (e is not OperationCanceledException) {
            result = Result.Fail(
                new ExceptionalError(
                    $"Error while updating #{PodType.ToPodName()} POD", e ) );
        }
        finally {
            Interlocked.Exchange( ref _interlocked, 0 );
        }

        return result;
    }

    private readonly INewsFetcher<TPotdNews> _newsFetcher;
    private readonly IPotdFetcher<TPotdNews> _potdFetcher;
    private readonly IResourceManager _resourceManager;

    private int _interlocked;
}
