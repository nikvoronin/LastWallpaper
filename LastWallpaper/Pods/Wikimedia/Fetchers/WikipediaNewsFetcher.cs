using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Wikimedia.Models;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Wikimedia.Fetchers;

public sealed class WikipediaNewsFetcher : INewsFetcher<WikipediaPodNews>
{
    public WikipediaNewsFetcher( HttpClient httpClient )
    {
        ArgumentNullException.ThrowIfNull( httpClient );

        _httpClient = httpClient;
    }

    public async Task<Result<WikipediaPodNews>> FetchNewsAsync( CancellationToken ct )
    {
        var nowDate = DateTime.Now.Date;

        var jsonPotdFilename =
            await _httpClient.GetFromJsonAsync<WmResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryTemplates.PotdFilenameFormat,
                    nowDate.ToString( "yyyy-MM-dd" ) ),
                ct );

        var supportedImageType =
            jsonPotdFilename?.Query.Pages[0].Images?[0].Value
            is string fileTitle
            && _supportedMediaTypesExtensions.Any(
                extension => fileTitle.EndsWith( extension, true, CultureInfo.InvariantCulture ) );

        if (jsonPotdFilename is null
            || (jsonPotdFilename.Query.Pages[0].Missing ?? false)
            || !supportedImageType)
            return Result.Fail( $"No updates for {nowDate}" );

        return Result.Ok(
            new WikipediaPodNews() {
                PodType = PodType.Wikipedia,
                PubDate = nowDate,
                Response = jsonPotdFilename
            } );
    }

    private readonly HttpClient _httpClient;
    private static readonly FrozenSet<string> _supportedMediaTypesExtensions =
            new HashSet<string> {
                ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi"
            }.ToFrozenSet();
}
