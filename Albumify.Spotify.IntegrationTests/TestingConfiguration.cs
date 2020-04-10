using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Albumify.Spotify.IntegrationTests
{
    internal class TestingConfiguration
    {
        private readonly Dictionary<string, string> OverriddenSettings = new Dictionary<string, string>();

        public TestingConfiguration WithWrongSpotifyClientId()
        {
            OverriddenSettings.Add("SpotifyClientId", "wrongSpotifyClientId");
            return this;
        }

        public TestingConfiguration WithWrongSpotifyClientSecret()
        {
            OverriddenSettings.Add("SpotifyClientSecret", "wrongSpotifyClientSecret");
            return this;
        }

        public IConfiguration Build()
        {
            return new ConfigurationBuilder()
                .AddUserSecrets("Albumify")
                .AddEnvironmentVariables()
                .AddInMemoryCollection(OverriddenSettings)
                .Build();
        }
    }
}
