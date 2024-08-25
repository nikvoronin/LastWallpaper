using FluentAssertions;
using FluentResults;
using HtmlAgilityPack;
using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using LastWallpaper.Pods.Natgeotv;
using Moq;

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

        var pod =
            new TestNatgeotvPodLoader(
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

    public const string _htmlPodPageFileName =
        "./samples/www.natgeotv.com_ca_photo-of-the-day.html";
}

public class TestNatgeotvPodLoader(
    HttpClient httpClient,
    IResourceManager resourceManager )
    : NatgeotvPodLoader(
        httpClient,
        resourceManager )
{
    public Result<HtmlPodNews> TestExtractHtmlDescription( HtmlNode documentNode ) =>
        ExtractHtmlDescription( documentNode );
}
