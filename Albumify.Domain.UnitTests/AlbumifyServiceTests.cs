using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Albumify.Domain.Models;

namespace Albumify.Domain.UnitTests
{
    [TestClass]
    public class TheAlbumifyService_WhenAddingAnAlbum
    {
        private const string ThirdPartyId = "test-album-id";
        private readonly ILogger<AlbumifyService> logger = new Mock<ILogger<AlbumifyService>>().Object;

        private Mock<I3rdPartyMusicService> thirdPartyMusicService;
        private Mock<IMyCollectionRepository> myCollection;
        private AlbumifyService sut;

        [TestInitialize]
        public void TestInitialize()
        {
            thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
            myCollection = new Mock<IMyCollectionRepository>();
            sut = new AlbumifyService(logger, thirdPartyMusicService.Object, myCollection.Object);
        }

        [TestMethod]
        public async Task GetsTheAlbumFromTheThirdPartyMusicService()
        {
            await sut.AddAsync(ThirdPartyId);
            thirdPartyMusicService.Verify(s => s.GetAlbumAsync(ThirdPartyId), Times.Once);
        }

        [TestMethod]
        public async Task AddsTheAlbumToMyCollection()
        {
            var thirdPartyMusicServiceAlbum = StubGettingAnAlbumFromThirdPartyMusicService();
            await sut.AddAsync(ThirdPartyId);
            myCollection.Verify(c => c.AddAsync(thirdPartyMusicServiceAlbum), Times.Once);
        }

        [TestMethod]
        public async Task ReturnsTheAddedAlbum()
        {
            StubGettingAnAlbumFromThirdPartyMusicService();
            var expected = StubAddingAnAlbumToMyCollection();
            var result = await sut.AddAsync(ThirdPartyId);
            result.Should().BeEquivalentTo(expected);
        }

        private Album StubGettingAnAlbumFromThirdPartyMusicService()
        {
            var thirdPartyMusicServiceAlbum = new Album { ThirdPartyId = ThirdPartyId };
            thirdPartyMusicService.Setup(s => s.GetAlbumAsync(It.IsAny<string>())).ReturnsAsync(thirdPartyMusicServiceAlbum);
            return thirdPartyMusicServiceAlbum;
        }

        private Album StubAddingAnAlbumToMyCollection()
        {
            var expected = new Album { Id = "collection-id", ThirdPartyId = ThirdPartyId };
            myCollection.Setup(c => c.AddAsync(It.IsAny<Album>())).ReturnsAsync(expected);
            return expected;
        }
    }
}