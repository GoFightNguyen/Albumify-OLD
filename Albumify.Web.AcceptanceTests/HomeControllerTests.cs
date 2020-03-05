using Albumify.Domain.Spotify;
using Albumify.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Albumify.Web.AcceptanceTests
{
    [TestClass]
    public class HomeControllerTests
        : WebApplicationFactory<Albumify.Web.Startup>
    {
        private WebApplicationFactory<Albumify.Web.Startup> _factory;

        [TestInitialize]
        public void TestInitialize()
            => _factory = new WebApplicationFactory<Startup>();

        [TestMethod]
        public async Task TestMethod1()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Index_AuthenticatesWithSpotify()
        {
            var spotifyMusicSource = new SpotifyMusicSource(new ConfigurationBuilder().AddUserSecrets("Albumify").Build());

            var logger = new Mock<ILogger<HomeController>>();
            var sut = new HomeController(logger.Object, spotifyMusicSource);
            var response = await sut.Index();
            Assert.IsInstanceOfType(response, typeof(ViewResult));
            var model = (response as ViewResult).Model as SpotifyAuthenticationResult;
            Assert.AreEqual(3600, model.ExpiresIn);
            Assert.AreEqual("Bearer", model.TokenType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(model.AccessToken));
        }
    }
}
