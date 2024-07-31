using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Handlers;

public sealed class PodsUpdateHandler(
    IReadOnlyCollection<IPotdLoader> _pods,
    IParameterizedUpdateHandler<FrontUpdateParameters> _frontUpdateHandler )
    : IAsyncUpdateHandler
{
    public async Task HandleUpdateAsync( CancellationToken ct )
    {
        var news = new Dictionary<string, Imago>();

        foreach (var pod in _pods) {
            ct.ThrowIfCancellationRequested();

            var result = await pod.UpdateAsync( ct );
            if (result.IsSuccess)
                news.TryAdd( pod.Name, result.Value );
        }

        // TODO: share news with Selector
        _frontUpdateHandler?.HandleUpdate(
            new(
                news.Count > 0,
                news.Values.FirstOrDefault() ),
            ct );
    }
}
