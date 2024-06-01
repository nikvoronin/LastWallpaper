using LastWallpaper.Models;
using LastWallpaper.Pods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper;

public sealed class Scheduler : IDisposable
{
    public Scheduler(
        IUpdateHandler updateHandler,
        IReadOnlyCollection<IPictureDayLoader> pods )
    {
        Debug.Assert( pods is not null );

        _pods = pods;
        _updateHandler = updateHandler;
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        Debug.Assert( _cts is not null );

        if (_timer is not null) return;

        _timer = new(
            OnTimerTick,
            null,
            StartImmediately,
            (long)CheckNewImagePeriod.TotalMilliseconds );
    }

    private void OnTimerTick( object? _ ) => Update();

    public void Update()
    {
        Debug.Assert( _cts is not null );

        var ct = _cts.Token;
        Task.Run( async () => {
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
                catch (Exception e) {
                    // TODO: log exception
                }
            }

            // TODO: share news with Selector
            if (news.Count > 0) {
                var first = news.Values.First();
                _updateHandler.HandleUpdate(first);
            }
        } );
    }

    public void Dispose()
    {
        try {
            _cts?.Cancel();

            // disable timer
            _timer?.Change( Timeout.Infinite, Timeout.Infinite );
            _timer = null;
        }
        catch { }
    }

    private Timer? _timer;
    private readonly IReadOnlyCollection<IPictureDayLoader> _pods;
    private readonly CancellationTokenSource _cts;
    private readonly IUpdateHandler _updateHandler;

    private const int StartImmediately = 0;

    private static readonly TimeSpan CheckNewImagePeriod = TimeSpan.FromMinutes( 57 );
}
