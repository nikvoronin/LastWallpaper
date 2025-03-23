using FluentResults;
using LastWallpaper.Abstractions;
using LastWallpaper.Models.Rss;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LastWallpaper.Logic;

public class RssReader : IFeedReader<RssFeed>
{
    public async Task<Result<RssFeed>> ParseFeedAsync(
        Uri uri,
        HttpClient httpClient,
        CancellationToken ct )
    {
        await using var stream = await httpClient.GetStreamAsync( uri, ct );
        var serializer = new XmlSerializer( typeof( RssFeed ) );
        var feed = serializer.Deserialize( stream ) as RssFeed;

        return
            feed is not null ? Result.Ok( feed )
            : Result.Fail( "Can not read RSS." );
    }
}
