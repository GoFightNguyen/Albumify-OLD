using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Albumify.Spotify.Models;

namespace Albumify.Spotify.UnitTests
{
    [TestClass]
    public class TheSpotifyWebApi_WhenGettingAnArtistsAlbums
    {
        [TestMethod]
        public async Task HandlesAlbums_WithAFullReleaseDateOrJustAYear()
        {
            // Arrange
            var spotifyAuthorization = StubAuthorization();
            var httpClient = StubHttpClient();

            // Act
            var sut = new SpotifyWebApi(httpClient, spotifyAuthorization);
            var result = await sut.GetAnArtistsAlbumsAsync("does not matter for this test");

            // Assert
            var expected = new List<string> { "2019-10-25", "2005" };
            result.Select(r => r.ReleaseDate).Should().BeEquivalentTo(expected);
        }

        private static ISpotifyAuthorization StubAuthorization()
        {
            var spotifyAuthorization = new Mock<ISpotifyAuthorization>();
            spotifyAuthorization
                .Setup(a => a.RequestAsync())
                .ReturnsAsync(new AuthenticationObject { AccessToken = "UnitTests", ExpiresIn = 3600, TokenType = "bearer" });
            return spotifyAuthorization.Object;
        }

        private static HttpClient StubHttpClient()
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();

            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                    )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(File.ReadAllText("AlbumsWithMixedReleaseDates.json"))
                });

            return new HttpClient(httpMessageHandler.Object);
        }
    }
}