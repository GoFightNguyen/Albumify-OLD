using Albumify.Domain.Spotify;
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

namespace Albumify.Domain.UnitTests
{
    [TestClass]
    public class TheSpotifyWebApi_WhenFindingAlbumsByArtist
    {
        [TestMethod]
        public async Task HandlesAlbums_WithAFullReleaseDateOrJustAYear()
        {
            // Arrange
            var spotifyAuthorization = StubAuthorization();
            var httpClient = StubHttpClient();

            // Act
            var sut = new SpotifyWebApi(httpClient, spotifyAuthorization);
            var result = await sut.FindAlbumsByArtistAsync("unit test");

            // Assert
            var expected1 = new SpotifySearchAlbumResult
            {
                Name = "Full Release Date",
                NumberOfSongs = 12,
                ReleaseDate = "2016-09-09"
            };
            var expected2 = new SpotifySearchAlbumResult
            {
                Name = "Year Only",
                NumberOfSongs = 7,
                ReleaseDate = "1978"
            };
            var expected = new List<string> { "2016-09-09", "1978" };
            result.Select(r => r.ReleaseDate).Should().BeEquivalentTo(expected);
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

        private static ISpotifyAuthorization StubAuthorization()
        {
            var spotifyAuthorization = new Mock<ISpotifyAuthorization>();
            spotifyAuthorization
                .Setup(a => a.RequestAsync())
                .ReturnsAsync(new SpotifyAuthorizationResult { AccessToken = "UnitTests", ExpiresIn = 3600, TokenType = "bearer" });
            return spotifyAuthorization.Object;
        }
    }

    // [TestClass]
    // public class TheSpotifyMusicSource_XYX
    // {
    //     [TestMethod]
    //     public async Task WillAuthenticate_WhenNotAlreadyAuthenticated()
    //     {
    //         // Arrange
    //         var config = new ConfigurationBuilder()
    //             .AddInMemoryCollection(new Dictionary<string, string>
    //             {
    //                 { "SpotifyClientId", "id" },
    //                 { "SpotifyClientSecret", "secret" }
    //             })
    //             .Build();
    //         var expected = new SpotifyAuthenticationResult { AccessToken = "UnitTests", ExpiresIn = 3600, TokenType = "bearer" };

    //         var httpMessageHandler = new Mock<HttpMessageHandler>();
    //         httpMessageHandler
    //             .Protected()
    //             .Setup<Task<HttpResponseMessage>>(
    //                 "SendAsync",
    //                 ItExpr.Is<HttpRequestMessage>(req => req.),
    //                 ItExpr.IsAny<CancellationToken>()

    ////                 "SendAsync",
    ////Times.Exactly(1), // we expected a single external request
    ////ItExpr.Is<HttpRequestMessage>(req =>
    ////   req.Method == HttpMethod.Get  // we expected a GET request
    ////   && req.RequestUri == expectedUri
    //                 )
    //             .ReturnsAsync(new HttpResponseMessage
    //             {
    //                 StatusCode = System.Net.HttpStatusCode.OK,
    //                 Content = new StringContent(JsonSerializer.Serialize(expected))
    //             });


    //         // Act
    //         var httpClient = new HttpClient(httpMessageHandler.Object);
    //         var sut = new SpotifyMusicSource3(config, httpClient);
    //         var result = await sut.AuthenticateUsingClientCredentialsFlowAsync();

    //         // Assert
    //     }
    // }

}