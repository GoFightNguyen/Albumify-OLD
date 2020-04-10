using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Albumify.Domain.IntegrationTests
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

        public TestingConfiguration WithWrongMongoDbUsername()
        {
            OverriddenSettings.Add("MongoDBUsername", "wrongMongoDBUsername");
            return this;
        }

        public TestingConfiguration WithWrongMongoDbPassword()
        {
            OverriddenSettings.Add("MongoDBPassword", "wrongMongoDBPassword");
            return this;
        }

        public TestingConfiguration WithWrongMongoDbHost()
        {
            OverriddenSettings.Add("MongoDBHost", "wrongMongoDBHost");
            return this;
        }

        public TestingConfiguration WithWrongMongoDbHostScheme()
        {
            OverriddenSettings.Add("MongoDBHostScheme", "wrongMongoDBHostScheme");
            return this;
        }

        public IConfiguration Build()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("Albumify")
                .AddEnvironmentVariables()
                .AddInMemoryCollection(OverriddenSettings)
                .Build();
        }
    }
}
