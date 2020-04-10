using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Albumify.MongoDB.IntegrationTests
{
    internal class TestingConfiguration
    {
        private readonly Dictionary<string, string> OverriddenSettings = new Dictionary<string, string>();

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
                .AddInMemoryCollection(OverriddenSettings)
                .Build();
        }
    }
}
