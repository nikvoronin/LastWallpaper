using FluentAssertions;
using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Astrobin;
using Moq;

namespace LastWallpaper.Tests.Pods.Astrobin;

public class AstrobinPodLoaderTests
{
    [Fact]
    public void CanExtractIotdInfo()
    {
        // Arrange
        var expected =
            new HtmlPodNews() {
                Author = "Henning Schmidt",
                Title = "High-Resolution-Animation of Saturn from 2018 to 2024",
                PubDate = new DateTime( 2024, 8, 10 ),
                Url = "https://www.astrobin.com/full/2i47ur/0/",
            };

        using var stream = File.OpenRead( _htmlArchiveFileName );
        var doc = new HtmlDocument();
        doc.Load( stream );
        var docNode = doc.DocumentNode;

        var pod =
            new TestAstrobinPodLoader(
                Mock.Of<HttpClient>(),
                Mock.Of<IResourceManager>() );

        // Act
        var actual = pod.TestExtractHtmlDescription( docNode );

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
        const string expected =
            "https://cdn.astrobin.com/thumbs/rS59lKRrZJEs_2560x0_esdlMP5Y.jpg";

        using var stream = File.OpenRead( _htmlHdImageFileName );
        var doc = new HtmlDocument();
        doc.Load( stream );
        var docNode = doc.DocumentNode;

        var pod =
            new TestAstrobinPodLoader(
                Mock.Of<HttpClient>(),
                Mock.Of<IResourceManager>() );

        // Act
        var actual = pod.TestExtractHdImageUrl( docNode );

        // Assert
        actual.Should().NotBeNull();

        actual.IsSuccess
            .Should().BeTrue();
        actual.Value
            .Should().Be( expected );
    }

    public const string _htmlArchiveFileName =
        "./samples/www.astrobin.com-iotd-archive.html";
    public const string _htmlHdImageFileName =
        "./samples/www.astrobin.com-full-FIGURE_HREF-0.html";
}

public class TestAstrobinPodLoader : AstrobinPodLoader
{
    public TestAstrobinPodLoader( HttpClient httpClient,
        IResourceManager resourceManager )
        : base( httpClient, resourceManager )
    { }

    public Result<HtmlPodNews> TestExtractHtmlDescription( HtmlNode documentNode ) =>
        ExtractHtmlDescription( documentNode );

    public Result<string> TestExtractHdImageUrl( HtmlNode documentNode ) =>
        ExtractHdImageUrl( documentNode );
}
