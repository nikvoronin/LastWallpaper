using LastWallpaper.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions;

public interface IResultsProcessor<TResults, TUpdateParameters>
    where TResults : IEnumerable<PodUpdateResult>
    where TUpdateParameters : FrontUpdateParameters
{
    ValueTask<TUpdateParameters> ProcessResultsAsync(
        TResults results,
        CancellationToken ct );
}
