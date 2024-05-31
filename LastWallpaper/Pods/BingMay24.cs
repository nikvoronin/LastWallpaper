using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public sealed class BingMay24 : PictureDayLoader
{
    public override string Name => nameof( BingMay24 );

    protected override async Task<IReadOnlyCollection<string>> UpdateInternalAsync(
        CancellationToken ct )
    {
        await Task.Delay( 10000, ct ); // TODO: replace with update+download process
        return [];
    }
}
