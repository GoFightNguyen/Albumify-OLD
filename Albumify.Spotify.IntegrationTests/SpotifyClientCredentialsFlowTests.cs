using Albumify.Spotify;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Albumify.Spotify.IntegrationTests
{
    [TestClass]
    public class TheSpotifyClientCredentialsFlow_WhenRequesting
    {
        [TestClass]
        public class ThrowsAnException
        {
            [TestMethod]
            public async Task WithAnInvalidClientId()
            {
                var config = new TestingConfiguration().WithWrongSpotifyClientId().Build();
                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientSecret()
            {
                var config = new TestingConfiguration().WithWrongSpotifyClientSecret().Build();
                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client secret. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientIdAndInvalidClientSecret()
            {
                var config = new TestingConfiguration()
                    .WithWrongSpotifyClientId()
                    .WithWrongSpotifyClientSecret()
                    .Build();
                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            private static async Task<Exception> TryToAuthenticate(IConfiguration config)
            {
                Exception thrownEx = null;
                try
                {
                    var sut = new SpotifyClientCredentialsFlow(config, new HttpClient());
                    await sut.RequestAsync();

                }
                catch (Exception ex)
                {
                    thrownEx = ex;
                }

                Assert.IsNotNull(thrownEx);
                Assert.IsInstanceOfType(thrownEx, typeof(SpotifyAuthorizationException));
                return thrownEx;
            }
        }

        [TestClass]
        public class SuccessfullyAuthorizes
        {
            [TestMethod]
            public async Task WithValidCredentials()
            {
                var config = new TestingConfiguration().Build();
                var sut = new SpotifyClientCredentialsFlow(config, new HttpClient());
                var result = await sut.RequestAsync();
                Assert.AreEqual(3600, result.ExpiresIn);
                Assert.AreEqual("Bearer", result.TokenType);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
            }
        }
    }
}
