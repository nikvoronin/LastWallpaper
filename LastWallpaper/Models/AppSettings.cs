using LastWallpaper.Pods.Bing.Models;
using LastWallpaper.Pods.Nasa.Models;
using LastWallpaper.Pods.Wikimedia.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LastWallpaper.Models;

public class AppSettings
{
    [JsonPropertyName( "toast_expire_in" )]
    public TimeSpan ToastExpireIn { get; init; } = TimeSpan.FromDays( 2 );

    [JsonPropertyName( "update_every" )]
    public TimeSpan UpdateEvery { get; init; } = TimeSpan.FromMinutes( 57 );
    
    [JsonPropertyName( "update_timeout" )]
    public TimeSpan SchedulerUpdateTimeout { get; init; } = TimeSpan.FromMinutes( 5 );

    [JsonPropertyName( "tray_icon" )]
    public TrayIconType TrayIcon { get; init; } = TrayIconType.Replica;

    [JsonPropertyName( "user_agent" )]
    public string UserAgent { get; init; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

    [JsonPropertyName( "active_pods" )]
    public IReadOnlyCollection<PodType> ActivePods { get; init; } = [];

    [JsonPropertyName( "bing" )]
    public BingSettings BingOptions { get; init; } = new();

    [JsonPropertyName( "apod" )]
    public ApodSettings ApodOptions { get; init; } = new();

    [JsonPropertyName( "wikipedia" )]
    public WikipediaSettings WikipediaOptions { get; init; } = new();
}
