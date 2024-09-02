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
        const string expectedUrl = "gopher://site.org/images/science/potd_diodon_2.jpeg";
        var expected = new Uri(expectedUrl);

        // Act
        var actual = ElementyPodLoader.ToHdImageUrl( sourceUrl );

        // Assert
        actual.Should().NotBeNull()
            .And.Be( expected );
    }
}