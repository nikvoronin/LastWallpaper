using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public interface IPictureDayLoader
{
    Task<IReadOnlyCollection<string>> UpdateAsync(
        CancellationToken ct );
}
