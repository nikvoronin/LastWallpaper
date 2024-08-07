using FluentAssertions;
using LastWallpaper.Logic;
using Moq;
using Moq.Protected;
using System.Net;

namespace LastWallpaper.Tests.Logic;

public class RssReaderTests
{
    public const string _xmlFileName = "./Models/Rss/elementy.xml";

    [Fact]
    public async void CanDownloadAndParseFeed()
    {
        // Arrange
        const string url = "url://abc.com";

        var httpMessageHandler = new Mock<HttpMessageHandler>( MockBehavior.Strict );

        httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>( x =>
                    x.Method == HttpMethod.Get
                    && x.RequestUri!.ToString().Contains( url ) ),
                ItExpr.IsAny<CancellationToken>() )
            .ReturnsAsync(
                new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent( File.OpenRead( _xmlFileName ) )
                } );

        var httpClient = new HttpClient( httpMessageHandler.Object );
        var rssReader = new RssReader();

        // Act
        var actual =
            await rssReader.ParseFeedAsync(
                url, httpClient, CancellationToken.None );

        // Assert
        actual.Should().NotBeNull();
        actual.Value.Should().NotBeNull();
        actual.Value.Channel.Should().NotBeNull();
        actual.Value.Channel.Items.Should().NotBeNullOrEmpty();
    }
}