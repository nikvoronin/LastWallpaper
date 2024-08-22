using FluentAssertions;
using LastWallpaper.Logic;

namespace LastWallpaper.Tests.Logic;

public class StringExtensionsTests
{
    [Theory]
    [InlineData( null, 250, null )]
    [InlineData( "0123456789", 4, "0123" )]
    [InlineData( "0123", 40, "0123" )]
    public void CanTruncate( string? input, int maxLength, string? expected )
    {
        // Act
        var actual = input!.Truncate( maxLength );

        // Assert
        actual.Should().Be( expected );
    }
}
