using System;
using System.Xml.Serialization;

namespace LastWallpaper.Models.Rss;

public sealed class RssItem
{
    [XmlElement( "enclosure" )]
    public RssEnclosure Enclosure { get; init; }

    [XmlElement( "category" )]
    public string Category { get; init; }

    [XmlElement( "description" )]
    public string Description { get; init; }

    [XmlElement( "link" )]
    public string Link { get; init; }

    [XmlElement( "pubDate" )]
    public string PubDateRaw { get; init; }

    [XmlIgnore]
    public DateTimeOffset PubDate => DateTimeOffset.Parse( PubDateRaw );

    [XmlElement( "title" )]
    public string Title { get; init; }
}
