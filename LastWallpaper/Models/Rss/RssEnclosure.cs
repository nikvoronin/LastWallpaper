using System.Xml.Serialization;

namespace LastWallpaper.Models.Rss;

public sealed class RssEnclosure
{
    [XmlAttribute( "type" )]
    public string Type { get; init; }

    [XmlAttribute( "url" )]
    public string Url { get; init; }
}
