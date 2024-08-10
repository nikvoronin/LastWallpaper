using FluentAssertions;
using LastWallpaper.Pods.Elementy;

namespace LastWallpaper.Tests.Pods.Elementy;

public class ElementyPodLoaderTests
{
    [Fact]
    public void CanParseUrlWithImageFile()
    {
        // Arrange
        const string sourceUrl = "gopher://site.org/images/science/potd_diodon_2_703.jpeg";
        const string expected = "gopher://site.org/images/science/potd_diodon_2.jpeg";

        // Act
        var actual = ElementyPodLoader.ConvertToHdFileUrl( sourceUrl );

        // Assert
        actual.Should().NotBeNullOrWhiteSpace()
            .And.Be( expected );
    }
}