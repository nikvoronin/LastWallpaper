using FluentAssertions;
using HtmlAgilityPack;
using LastWallpaper.Pods.Astrobin;
using LastWallpaper.Pods.Astrobin.Models;
using System.Globalization;

namespace LastWallpaper.Tests.Pods.Elementy;

public class AstrobinPodLoaderTests
{
    [Fact]
    public void CanExtractIotdInfo()
    {
        // Arrange
        var expected =
            new AbinIotdDescription() {
                Author = "Henning Schmidt",
                Title = "High-Resolution-Animation of Saturn from 2018 to 2024",
                PubDate =
                    DateTime.ParseExact(
                        "08/10/2024", "MM/dd/yyyy",
                        CultureInfo.InvariantCulture ),
                HdPageUrl = "https://www.astrobin.com/full/2i47ur/0/",
            };

        using var stream = File.OpenRead( _htmlArchiveFileName );
        var doc = new HtmlDocument();
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

    [Fact]
    public void CanExtractFullImageUrl()
    {
        // Arrange
        var expected = "https://cdn.astrobin.com/thumbs/rS59lKRrZJEs_2560x0_esdlMP5Y.jpg";

        using var stream = File.OpenRead( _htmlFullImageFileName );
        var doc = new HtmlDocument();
        doc.Load( stream );
        var docNode = doc.DocumentNode;

        // Act
        var actual = AstrobinPodLoader.ExtractHdImageUrl( docNode );

        // Assert
        actual.Should().NotBeNull();

        actual.IsSuccess
            .Should().BeTrue();
        actual.Value
            .Should().Be( expected );
    }

    public const string _htmlArchiveFileName =
        "./samples/www.astrobin.com-iotd-archive.html";
    public const string _htmlFullImageFileName =
        "./samples/www.astrobin.com-full-FIGURE_HREF-0.html";
}