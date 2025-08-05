using FluentResults;
using LastWallpaper.Abstractions.Fetchers;
using LastWallpaper.Models;
using LastWallpaper.Pods.Apod.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Apod.Fetchers;

public sealed class ApodDescriptionFetcher : IDescriptionFetcher<ApodNews>
{
    public Task<Result<PotdDescription>> FetchDescriptionAsync(
        ApodNews news,
        CancellationToken ct )
    {
        var imageInfo = news.Description;

        var owner =
            imageInfo.Copyright
                ?.Trim().Replace( "\n", "" )
                ?? "(cc) Public domain";

        return
            Task.FromResult(
                imageInfo.HdImageUrl is null ? Result.Fail( "Can not find an hd-image-url." )
                : Result.Ok(
                    new PotdDescription() {
                        PodType = PodType.Apod,
                        Url = new Uri( imageInfo.HdImageUrl ),
                        PubDate = news.PubDate,
                        Title = imageInfo.Title,
                        Copyright = owner,
                    } ) );
    }
}
