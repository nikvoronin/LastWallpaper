using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#if !DEBUG
using System.Threading.Tasks;
#endif

namespace LastWallpaper.Handlers;

public sealed class PodsUpdateHandler(
    IReadOnlyCollection<IPotdLoader> _pods,
    IParameterizedUpdateHandler<Imago> _frontUpdateHandler )
    : IUpdateHandler
{
    public async void HandleUpdate( CancellationToken ct )
    {
        var news = new Dictionary<string, Imago>();

        foreach (var pod in _pods) {
            try {
                ct.ThrowIfCancellationRequested();

                var result = await pod.UpdateAsync( ct );
                if (result.IsSuccess)
                    news.TryAdd( pod.Name, result.Value );
            }
            catch (OperationCanceledException) {
                break;
            }
        }

        // TODO: share news with Selector
        if (news.Count > 0) {
            var imago = news.Values.First();
            _frontUpdateHandler?.HandleUpdate( imago, ct );
        }
    }
}
