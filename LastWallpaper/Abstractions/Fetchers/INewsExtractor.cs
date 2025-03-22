using FluentResults;
using HtmlAgilityPack;
namespace LastWallpaper.Abstractions.Fetchers;

public interface INewsExtractor<TPotdNews>
    where TPotdNews : IPotdNews
{
    Result<TPotdNews> ExtractNews( HtmlNode root );
}
