using FluentAssertions;
using LastWallpaper.Models.Rss;
using System.Xml.Serialization;

namespace LastWallpaper.Tests.Models.Rss;

public class RssFeedTests
{
    public const string _xmlFileName = "./samples/elementy.xml";

    [Fact]
    public void CanBeDeserialized()
    {
        // Arrange
        var serializer = new XmlSerializer( typeof( RssFeed ) );
        using var stream = File.OpenRead( _xmlFileName );

        // Act
        var actual = serializer.Deserialize( stream );

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<RssFeed>();
    }

    [Fact]
    public void CanReflectRssXml()
    {
        // Arrange
        var serializer = new XmlSerializer( typeof( RssFeed ) );
        using var stream = File.OpenRead( _xmlFileName );

        // Act
        var actual = (RssFeed)serializer.Deserialize( stream )!;

        // Assert
        actual.Should().NotBeNull();

        actual.Channel
            .Should().NotBeNull();
        actual.Channel.Items
            .Should().NotBeNullOrEmpty();

        var item = actual.Channel.Items[0];

        item.Title
            .Should().NotBeNullOrWhiteSpace();

        item.PubDate.Year
            .Should().BeGreaterThan( 2000 );

        item.Enclosure
            .Should().NotBeNull();
        item.Enclosure.Type
            .Should().NotBeNullOrEmpty();
        item.Enclosure.Url
            .Should().NotBeNullOrEmpty();
    }
}