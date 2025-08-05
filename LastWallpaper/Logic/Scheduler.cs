using LastWallpaper.Abstractions.Handlers;
using LastWallpaper.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic;

public sealed class Scheduler : IDisposable
{
    public Scheduler(
        IAsyncUpdateHandler updateHandler,
        AppSettings settings )
    {
        Debug.Assert( updateHandler is not null );

        _updatePodsPeriod = settings.UpdateEvery;
        _podsUpdateTimeout = settings.PodsUpdateTimeout;

        _updateHandler = updateHandler;
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        Debug.Assert( _cts is not null );

        if (_timer is not null) return;

        _timer = new(
            _ => UpdateInternal(),
            null,
            StartImmediately,
            (long)_updatePodsPeriod.TotalMilliseconds );
    }

    public void Update() => Task.Run( UpdateInternal );

    private async void UpdateInternal()
    {
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return;

        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource( _cts.Token );
        timeoutCts.CancelAfter( _podsUpdateTimeout );

        try {
            await _updateHandler.HandleUpdateAsync( timeoutCts.Token );
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested) { }
        finally {
            Interlocked.Exchange( ref _interlocked, 0 );
        }
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
    private int _interlocked;
    private readonly CancellationTokenSource _cts;
    private readonly IAsyncUpdateHandler _updateHandler;
    private readonly TimeSpan _updatePodsPeriod;
    private readonly TimeSpan _podsUpdateTimeout;

    private const int StartImmediately = 0;
}
