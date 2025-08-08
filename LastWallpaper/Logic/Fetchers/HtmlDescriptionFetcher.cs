using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Fetchers;

public sealed class HtmlDescriptionFetcher : IDescriptionFetcher<HtmlPodNews>
{
    public HtmlDescriptionFetcher()
    {
        _options = new();
    }

    public HtmlDescriptionFetcher( HtmlDescriptionFetcherOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );

        _options = options;
    }

    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        HtmlPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                PodType = news.PodType,
                Url = new( news.Url ),
                PubDate = news.PubDate,
                Title = news.Title,
                Copyright = 
                    _options.UseCopyrightSign ? $"© {news.Author}"
                    : news.Author,
            } ) );

    private readonly HtmlDescriptionFetcherOptions _options;
}
