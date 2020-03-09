using Albumify.Domain.Spotify;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Albumify.Domain.UnitTests
{
    [TestClass]
    public class TheSpotifyClientCredentialsFlow_WhenRequesting
    {
        private static readonly SpotifyAuthorizationResult expected = 
            new SpotifyAuthorizationResult { AccessToken = "UnitTests", ExpiresIn = 3600, TokenType = "bearer" };

        [TestMethod]
        public async Task AuthorizesWithSpotify_IfNotAuthorized()
        {
            // Arrange
            var config = StubConfiguration();
            var httpMessageHandler = StubHttpCallForAuthorizationToReturn(expected);
            var httpClient = new HttpClient(httpMessageHandler.Object);

            // Act
            var sut = new SpotifyClientCredentialsFlow(config, httpClient);
            var result = await sut.RequestAsync();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task UsesCachedAuthorization_IfAlreadyAuthorized()
        {
            // Arrange
            var config = StubConfiguration();
            var httpMessageHandler = StubHttpCallForAuthorizationToReturn(expected);
            var httpClient = new HttpClient(httpMessageHandler.Object);

            // Act
            var sut = new SpotifyClientCredentialsFlow(config, httpClient);
            await sut.RequestAsync();
            var result = await sut.RequestAsync();

            // Assert
            result.Should().BeEquivalentTo(expected);
            httpMessageHandler
                .Protected()
                .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        private static IConfiguration StubConfiguration()
            => new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                     { "SpotifyClientId", "id" },
                     { "SpotifyClientSecret", "secret" }
                })
                .Build();

        private static Mock<HttpMessageHandler> StubHttpCallForAuthorizationToReturn(SpotifyAuthorizationResult expected)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                    )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expected))
                });
            return httpMessageHandler;
        }
    }

}