using FluentResults;
using HtmlAgilityPack;

namespace LastWallpaper.Abstractions.Fetchers;

/// <summary>
/// Defines an interface for extracting news related to "Picture of the Day" (PotD) from an HTML document.
/// </summary>
/// <typeparam name="TPotdNews">
/// The type of news data to extract, which must implement the <see cref="IPotdNews"/> interface.
/// </typeparam>
/// <remarks>
/// This interface is designed to work with HTML content parsed by the <see cref="HtmlAgilityPack"/> library.
/// It provides a method to extract news data from a given HTML node and returns the result wrapped in a <see cref="Result{T}"/> from the FluentResults library.
/// </remarks>
public interface INewsExtractor<TPotdNews>
    where TPotdNews : IPotdNews
{
    /// <summary>
    /// Extracts news data from the provided HTML node.
    /// </summary>
    /// <param name="root">
    /// The root <see cref="HtmlNode"/> from which to extract the news data.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the extracted news data of type <typeparamref name="TPotdNews"/>.
    /// If the extraction fails, the result will indicate the error.
    /// </returns>
    Result<TPotdNews> ExtractNews( HtmlNode root );
}
