using LastWallpaper.Abstractions;
using System;
using System.Text.Json.Serialization;

namespace LastWallpaper.Pods.Nasa.Models;

public class ApodSettings : IPotdLoaderSettings
{
    [JsonPropertyName("throttling_hours")]
    public TimeSpan ThrottlingHours { get; init; } = TimeSpan.FromHours(23);

    [JsonPropertyName("api_key")]
    public string ApiKey { get; init; } = DefaultApiKey;

    public const string DefaultApiKey = "DEMO_KEY";
}
