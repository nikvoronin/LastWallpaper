using FluentResults;
using LastWallpaper.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PictureDayLoader : IPictureDayLoader
{
    public abstract string Name { get; }

    protected PictureDayLoader( HttpClient client )
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
