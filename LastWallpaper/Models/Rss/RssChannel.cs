using System.Xml.Serialization;

namespace LastWallpaper.Models.Rss;

public sealed class RssChannel
{
    [XmlElement( "title" )]
    public string Title { get; init; }

    [XmlElement( "link" )]
    public string Link { get; init; }

    [XmlElement( "description" )]
    public string Description { get; init; }

    [XmlElement( "lastBuildDate" )]
    public string LastBuildDate { get; init; }

    [XmlElement( "image" )]
    public object? Image { get; init; }

    [XmlElement( "item" )]
    public RssItem[] Items { get; init; }
}
