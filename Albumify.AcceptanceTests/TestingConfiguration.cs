using Microsoft.Extensions.Configuration;

namespace Albumify.AcceptanceTests
{
    internal class TestingConfiguration
    {
        public IConfiguration Build()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("Albumify")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
