using System.Xml.Serialization;

namespace LastWallpaper.Models.Rss;

[XmlRoot( "rss" )]
public sealed class RssFeed
{
    [XmlElement( "channel" )]
    public required RssChannel Channel { get; init; }
}
