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
    protected readonly HttpClient _client;
    protected readonly IPotdLoaderSettings _settings;

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
