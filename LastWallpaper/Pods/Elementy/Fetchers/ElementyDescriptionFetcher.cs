using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Elementy.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Elementy.Fetchers;

public sealed class ElementyDescriptionFetcher : IDescriptionFetcher<ElementyPodNews>
{
    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        ElementyPodNews news,
        CancellationToken ct )
        => Task.FromResult( Result.Ok(
            new PotdDescription() {
                PodType = PodType.Elementy,
                Url = ToHdImageUrl( news.Item.Enclosure.Url ),
                PubDate = news.Item.PubDate.Date,
                Title = $"{news.Item.Title}. {news.Item.Category}.",
                Copyright = news.Item.Description,
            } ) );

    public static Uri ToHdImageUrl( string url )
    {
        var uri = new Uri( url );
        var filename = uri.Segments[^1];
        var hdFilename =
            filename[..filename.LastIndexOf( '_' )]
            + new FileInfo( filename ).Extension;

        return new Uri(
            $"{uri.Scheme}://{uri.Host}{string.Concat( uri.Segments[..^1] )}{hdFilename}" );
    }
}
