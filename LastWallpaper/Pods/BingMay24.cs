using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public class BingMay24 : IPictureDayLoader
{
    public async Task<IReadOnlyCollection<string>> UpdateAsync(
        CancellationToken ct )
    {
        throw new NotImplementedException();
    }
}
