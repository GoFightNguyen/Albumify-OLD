using Albumify.Domain.Spotify;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain.IntegrationTests
{
    [TestClass]
    public class TheSpotifyMusicSource
    {
        [TestClass]
        public class ThrowsAnException_WhenAuthenticatingUsingClientCredentialsFlow
        {
            [TestMethod]
            public async Task WithAnInvalidClientId()
            {
                var config = new ConfigurationBuilder()
                .AddUserSecrets("Albumify")
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "SpotifyClientId", "wrong" },
                })
                .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientSecret()
            {
                var config = new ConfigurationBuilder()
                    .AddUserSecrets("Albumify")
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "SpotifyClientSecret", "wrong" },
                    })
                    .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client secret. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientIdAndInvalidClientSecret()
            {
                var config = new ConfigurationBuilder()
                    .AddUserSecrets("Albumify")
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "SpotifyClientId", "wrong" },
                        { "SpotifyClientSecret", "alsoWrong" }
                    })
                    .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            private static async Task<Exception> TryToAuthenticate(IConfigurationRoot config)
            {
                Exception thrownEx = null;
                try
                {
                    var sut = new SpotifyMusicSource(config);
                    await sut.AuthenticateUsingClientCredentialsFlowAsync();

                }
                catch (Exception ex)
                {
                    thrownEx = ex;
                }

                Assert.IsNotNull(thrownEx);
                Assert.IsInstanceOfType(thrownEx, typeof(SpotifyAuthenticationException));
                return thrownEx;
            }
        }

        [TestClass]
        public class SuccessfullyAuthenticatesUsingClientCredentialsFlow
        {
            [TestMethod]
            public async Task WithValidCredentials()
            {
                var config = new ConfigurationBuilder().AddUserSecrets("Albumify").Build();
                var sut = new SpotifyMusicSource(config);
                var result = await sut.AuthenticateUsingClientCredentialsFlowAsync();
                Assert.AreEqual(3600, result.ExpiresIn);
                Assert.AreEqual("Bearer", result.TokenType);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
            }
        }
    }
}
