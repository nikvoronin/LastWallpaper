using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LastWallpaper.Pods.Wikimedia.Models;

public static class WmMetaDataExtensions
{
    public static string? ObjectName(
        this IReadOnlyDictionary<string, WmMetaData> data)
        => Untagify(
            data[nameof(ObjectName)]
            ?.Value
            .GetString());

    public static string? ImageDescription(
        this IReadOnlyDictionary<string, WmMetaData> data)
        => Untagify(
            data[nameof(ImageDescription)]
            ?.Value
            .GetString());

    public static string? Credit(
        this IReadOnlyDictionary<string, WmMetaData> data)
        => Untagify(
            data[nameof(Credit)]
            ?.Value
            .GetString());

    public static string? Artist(
        this IReadOnlyDictionary<string, WmMetaData> data)
        => Untagify(
            data[nameof(Artist)]
            ?.Value
            .GetString());

    public static string? Untagify(string? html)
    {
        if (html is null) return null;

        _doc.LoadHtml(html);

        return
            WebUtility.HtmlDecode(
                string.Concat(
                    _doc.DocumentNode
                    .DescendantsAndSelf()
                    .Select(node =>
                        node.HasChildNodes ? string.Empty
                        : node.InnerText
                            ?.Replace("\\n", string.Empty)
                            .Replace("\n", string.Empty)
                    )
                ));
    }

    private static readonly HtmlDocument _doc = new();
}