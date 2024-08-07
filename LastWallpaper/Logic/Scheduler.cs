﻿using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Diagnostics;
using System.Threading;

namespace LastWallpaper.Logic;

public sealed class Scheduler : IDisposable
{
    public Scheduler(
        IAsyncUpdateHandler updateHandler,
        AppSettings settings )
    {
        Debug.Assert( updateHandler is not null );

        _checkImageUpdateAfterPeriod = settings.UpdateEvery;
        _podsUpdateTimeout = settings.SchedulerUpdateTimeout;

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
    private readonly TimeSpan _checkImageUpdateAfterPeriod;
    private readonly TimeSpan _podsUpdateTimeout;

    private const int StartImmediately = 0;
}
