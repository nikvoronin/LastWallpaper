using FluentAssertions;
using HtmlAgilityPack;
using LastWallpaper.Pods.Astrobin;
using LastWallpaper.Pods.Astrobin.Models;
using System.Globalization;

namespace LastWallpaper.Tests.Pods.Elementy;

public class AstrobinPodLoaderTests
{
    public const string _htmlFileName = "./samples/www.astrobin.com-iotd-archive.html";

    [Fact]
    public void CanExtractIotdInfo()
    {
        // Arrange
        var expected =
            new AbinIotdDescription() {
                Author = "Henning Schmidt",
                Title = "High-Resolution-Animation of Saturn from 2018 to 2024",
                PubDate =
                    DateTimeOffset.ParseExact(
                        "08/10/2024", "MM/dd/yyyy",
                        CultureInfo.InvariantCulture ),
                HdPageUrl = "https://www.astrobin.com/full/2i47ur/0/",
            };

        using var stream = File.OpenRead( _htmlFileName );
        HtmlDocument doc = new();
        doc.Load( stream );
        var docNode = doc.DocumentNode;

        // Act
        var actual = AstrobinPodLoader.ExtractIotdInfo( docNode );

        // Assert
        actual.Should().NotBeNull();

        actual.IsSuccess
            .Should().BeTrue();
        actual.Value
            .Should().BeEquivalentTo( expected );
    }
}