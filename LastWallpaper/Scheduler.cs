using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper;

public sealed class Scheduler : IDisposable
{
    public Scheduler(
        IUpdateHandler updateHandler,
        IReadOnlyCollection<IPotdLoader> pods,
        AppSettings settings)
    {
        Debug.Assert( updateHandler is not null );
        Debug.Assert( pods is not null );

        _checkImageUpdateAfterPeriod = settings.UpdateEvery;
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
            (long)_checkImageUpdateAfterPeriod.TotalMilliseconds );
    }

    private void OnTimerTick( object? _ ) => Update();

    public void Update() =>
        Task.Run( () => _updateHandler.HandleUpdate( _cts.Token ) );

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
    private readonly CancellationTokenSource _cts;
    private readonly IUpdateHandler _updateHandler;
    private readonly TimeSpan _checkImageUpdateAfterPeriod;

    private const int StartImmediately = 0;
}
