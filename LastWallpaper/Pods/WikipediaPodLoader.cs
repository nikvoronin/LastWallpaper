using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods.Wikimedia;

public sealed class WikipediaPodLoader(
    HttpClient client,
    WikipediaSettings settings )
    : PodLoader( PodType.Wikipedia, client, settings )
{
    public override WikipediaSettings Settings => (WikipediaSettings)_settings;

    protected override async Task<Result<Imago>> UpdateInternalAsync(
        CancellationToken ct )
    {
        var jsonPotdFilename =
            await _client.GetFromJsonAsync<WikiMediaResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdFilenameFormat,
                    DateTime.Now.Date.ToString( "yyyy-MM-dd" ) ),
                ct );

        var potdFilename = jsonPotdFilename?.Query.Pages[0].Images?[0].Value;

        var jsonPotdImageLink =
            await _client.GetFromJsonAsync<WikiMediaResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdUrlFormat,
                    potdFilename ),
                ct );

        var potdImageDownloadLink = jsonPotdImageLink?.Query.Pages[0].ImageInfos?[0].Url;

        if (potdImageDownloadLink is null) return Result.Fail( "No image url were found." );

        await using var imageStream =
            await _client.GetStreamAsync( potdImageDownloadLink, ct );

        var filename = $"{Name}{DateTime.Now:yyyyMMdd}.jpeg";

        var imageFilename =
            Path.Combine(
                FileManager.AlbumFolder,
                filename );

        // TODO: check here should we load picture or picture is already known
        if (File.Exists( imageFilename )) return Result.Fail( "Picture already known." );

        await using var fileStream =
            new FileStream(
                imageFilename,
                FileMode.Create );

        await imageStream.CopyToAsync( fileStream, ct );

        var jsonPotdCredits =
            await _client.GetFromJsonAsync<WikiMediaResponse>(
                string.Format(
                    CultureInfo.InvariantCulture,
                    WmQueryPotdCreditsFormat,
                    potdFilename ),
                ct );

        // TODO: make it safe, should control nullable values
        var extMetaData = jsonPotdCredits?.Query.Pages[0].ImageInfos[0].ExtMetaData;

        var objectName = extMetaData.ObjectName();
        var fullDescription = extMetaData.ImageDescription();

        var description = fullDescription;
        if (description?.Length > 200) {
            var dotIndex = description.IndexOf( ". " );
            if (dotIndex > -1)
                description = description[..dotIndex];
        }

        var artist = extMetaData.Artist();
        var credit = extMetaData.Credit();

        var title =
            description.Length < 200 ? description
            : objectName;
        var copyrights =
            artist.Length + credit.Length > 100 ? artist
            : $"{artist}/{credit}";

        var result = new Imago() {
            PodName = Name,
            Filename = imageFilename,
            Created = DateTime.Now,
            Title = title,
            Copyright = copyrights,
            Description = fullDescription
        };

        return Result.Ok( result );
    }

    private static readonly CompositeFormat WmQueryPotdFilenameFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=images&titles=Template:POTD/{0}" );

    private static readonly CompositeFormat WmQueryPotdUrlFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=imageinfo&iiprop=url&titles={0}" );

    private static readonly CompositeFormat WmQueryPotdCreditsFormat =
        CompositeFormat.Parse(
            WikiMediaQueryBase + "&prop=imageinfo&iiprop=extmetadata&titles={0}" );

    private const string WikiMediaQueryBase =
        "https://en.wikipedia.org/w/api.php?action=query&format=json&formatversion=2";
}

public sealed class WikiMediaResponse
{
    [JsonPropertyName( "query" )]
    public required WmQuery Query { get; init; }

    public sealed class WmQuery
    {
        [JsonPropertyName( "pages" )]
        public required IReadOnlyList<WmPagedPotds> Pages { get; init; }
    }

    public sealed class WmPagedPotds
    {
        [JsonPropertyName( "pageid" )]
        public required long PageId { get; init; }

        [JsonPropertyName( "title" )]
        public required string Title { get; init; }

        [JsonPropertyName( "images" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public IReadOnlyList<ImageFilename>? Images { get; init; }

        [JsonPropertyName( "imageinfo" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public IReadOnlyList<ImageInfo>? ImageInfos { get; init; }

        public sealed class ImageFilename
        {
            [JsonPropertyName( "title" )]
            public required string Value { get; init; }
        }

        public sealed class ImageInfo
        {
            [JsonPropertyName( "url" )]
            [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
            public string? Url { get; init; }

            [JsonPropertyName( "extmetadata" )]
            [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
            public IReadOnlyDictionary<string, MetaData>? ExtMetaData { get; init; }
        }
    }
}

public class WikipediaSettings : IPotdLoaderSettings
{
}

public sealed class MetaData
{
    [JsonPropertyName( "value" )] public JsonElement Value { get; init; }
}

public static class ExtMetaDataExtensions
{
    public static string? ObjectName(
        this IReadOnlyDictionary<string, MetaData> data )
        => Untagify(
            data[nameof( ObjectName )]
            ?.Value
            .GetString() );

    public static string? ImageDescription(
        this IReadOnlyDictionary<string, MetaData> data )
        => Untagify(
            data[nameof( ImageDescription )]
            ?.Value
            .GetString() );

    public static string? Credit(
        this IReadOnlyDictionary<string, MetaData> data )
        => Untagify(
            data[nameof( Credit )]
            ?.Value
            .GetString() );

    public static string? Artist(
        this IReadOnlyDictionary<string, MetaData> data )
        => Untagify(
            data[nameof( Artist )]
            ?.Value
            .GetString() );

    public static string? Untagify( string? html )
    {
        if (html is null) return null;

        _doc.LoadHtml( html );

        return
            WebUtility.HtmlDecode(
                string.Concat(
                    _doc.DocumentNode
                    .DescendantsAndSelf()
                    .Select( node =>
                        node.HasChildNodes ? string.Empty
                        : node.InnerText
                            ?.Replace( "\\n", string.Empty )
                            .Replace( "\n", string.Empty )
                    )
                ) );
    }

    private static readonly HtmlDocument _doc = new();
}