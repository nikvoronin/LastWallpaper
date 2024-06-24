using FluentResults;
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
        IAsyncUpdateHandler updateHandler,
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
            _ => Update(),
            null,
            StartImmediately,
            (long)_checkImageUpdateAfterPeriod.TotalMilliseconds );
    }

    public async void Update()
    {
        if (Interlocked.CompareExchange( ref _interlocked, 1, 0 ) != 0)
            return;

        try {
            await _updateHandler.HandleUpdateAsync( _cts.Token );
        }
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
    private readonly TimeSpan _checkImageUpdateAfterPeriod;

    private const int StartImmediately = 0;
}
