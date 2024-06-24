using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PodLoader : IPotdLoader
{
    public string Name { get; }
    public abstract IPotdLoaderSettings Settings { get; }

    public async Task<Result<Imago>> UpdateAsync( CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return Result.Fail( "Update already in progress." );

        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource( ct );
        timeoutCts.CancelAfter( UpdateBypassTimeout );

        Result<Imago> result;
        try {
            result = await UpdateInternalAsync( timeoutCts.Token );
        }
        catch (Exception e)
        when (
            e is not OperationCanceledException
            || timeoutCts.IsCancellationRequested )
        {
            result = Result.Fail(
                new ExceptionalError(
                    $"Error while updating #{Name} POD", e ) );
        }
        finally {
            Interlocked.Exchange( ref _interlocked, 0 );
        }

        return result;
    }

    protected abstract Task<Result<Imago>> UpdateInternalAsync( CancellationToken ct );

    private int _interlocked;
    protected readonly HttpClient _client;
    protected readonly IPotdLoaderSettings _settings;

    private static readonly TimeSpan UpdateBypassTimeout = 
        TimeSpan.FromMinutes( 5 );

    protected PodLoader(
        PodType superType,
        HttpClient client,
        IPotdLoaderSettings settings )
    {
        _client = client;
        _settings = settings;

        Name = superType.ToString().ToLower();
    }
}
