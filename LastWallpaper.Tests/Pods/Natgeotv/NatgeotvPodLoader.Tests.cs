using FluentAssertions;
using HtmlAgilityPack;
using LastWallpaper.Models;
using LastWallpaper.Pods.Natgeotv;

namespace LastWallpaper.Tests.Pods.Natgeotv;

public class NatgeotvPodLoaderTests
{
    [Fact]
    public void CanExtractPotdInfo()
    {
        // Arrange
        var expected =
            new HtmlPodNews() {
                Author = "Blink Films UK",
                Title = "Aerospace engineer Sophie Harker looks at Apollo CGI. This is from Disaster Autopsy.",
                PubDate = new DateTime( 2024, 8, 22 ),
                Url = "https://assets-natgeotv.fnghub.com/POD/15483.jpg",
            };

        using var stream = File.OpenRead( _htmlPodPageFileName );
        var doc = new HtmlDocument();
        doc.Load( stream );
        var docNode = doc.DocumentNode;

        // Act
        var actual = NatgeotvPodLoader.ExtractPotdInfo( docNode );

        // Assert
        actual.Should().NotBeNull();

        actual.IsSuccess
            .Should().BeTrue();
        actual.Value
            .Should().BeEquivalentTo( expected );
    }

    public const string _htmlPodPageFileName =
        "./samples/www.natgeotv.com_ca_photo-of-the-day.html";
}