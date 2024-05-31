using LastWallpaper.Pods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper;

public sealed class Scheduler : IDisposable
{
    private Timer? _timer;
    private readonly IReadOnlyCollection<IPictureDayLoader> _pods;
    private readonly CancellationTokenSource _cts;

    public Scheduler(
        IReadOnlyCollection<IPictureDayLoader> pods )
    {
        Debug.Assert( pods is not null );

        _pods = pods;
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        Debug.Assert(_cts is not null );

        if (_timer is not null) return;

        _timer = new(
            OnTimerTick,
            _cts,
            StartImmediately,
            (long)CheckNewImagePeriod.TotalMilliseconds );
    }

    private void OnTimerTick( object? cts ) =>
        Update((cts as CancellationTokenSource)!.Token);

    public void Update( CancellationToken ct )
    {
        Task.Run( async () => {
            foreach (var pod in _pods) {
                try {
                    ct.ThrowIfCancellationRequested();

                    await pod.UpdateAsync( ct );
                }
                catch (OperationCanceledException) {
                    break;
                }
                catch (Exception e) {
                    // TODO: log exception
                }
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

    private const int StartImmediately = 0;

    private static readonly TimeSpan CheckNewImagePeriod = TimeSpan.FromMinutes( 57 );
}
