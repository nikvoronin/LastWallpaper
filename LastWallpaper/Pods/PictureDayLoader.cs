using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public abstract class PictureDayLoader : IPictureDayLoader
{
    public abstract string Name { get; }

    public async Task<IReadOnlyCollection<string>> UpdateAsync(
        CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return [];

        var result = await UpdateInternalAsync( ct );

        Interlocked.Exchange( ref _interlocked, 0 );
        return result;
    }

    protected abstract Task<IReadOnlyCollection<string>> UpdateInternalAsync(
        CancellationToken ct );

    private int _interlocked;
}
