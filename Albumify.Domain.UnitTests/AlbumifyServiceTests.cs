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
        [TestClass]
        public class IfAlreadyInMyCollection
        {
            private const string ThirdPartyId = "test-album-id";
            private static readonly Album CollectionAlbum = new Album
            {
                Id = "123-456-789",
                Label = "Unit Test",
                ThirdPartyId = ThirdPartyId
            };

            private static Mock<I3rdPartyMusicService> thirdPartyMusicService;
            private static Mock<IMyCollectionRepository> myCollection;
            private static Album result;

            [ClassInitialize]
            public static async Task ClassIniitalize(TestContext _)
            {
                var logger = new Mock<ILogger<AlbumifyService>>().Object;
                myCollection = new Mock<IMyCollectionRepository>();
                thirdPartyMusicService = new Mock<I3rdPartyMusicService>();

                myCollection.Setup(m => m.FindBy3rdPartyId(ThirdPartyId)).ReturnsAsync(CollectionAlbum);

                var sut = new AlbumifyService(logger, thirdPartyMusicService.Object, myCollection.Object);
                result = await sut.AddAsync(ThirdPartyId);
            }

            [TestMethod]
            public void DoesNotQuery3rdPartyMusicService() => thirdPartyMusicService.Verify(s => s.GetAlbumAsync(It.IsAny<string>()), Times.Never);

            [TestMethod]
            public void DoesNotAddToMyCollection() => myCollection.Verify(c => c.AddAsync(It.IsAny<Album>()), Times.Never);

            [TestMethod]
            public void ReturnsTheAlbumFromMyCollection() => result.Should().BeEquivalentTo(CollectionAlbum);
        }

        [TestClass]
        public class IfNotInMyCollection_ButInTheThirdPartyMusicService
        {
            private const string ThirdPartyId = "test-album-id";
            private static readonly Album ThirdPartyAlbum = new Album
            {
                Label = "Unit Test",
                ThirdPartyId = ThirdPartyId,
                Name = "Tested"
            };
            private static readonly Album ExpectedAddedAlbum = new Album
            {
                Id = "collection-id",
                Label = "Unit Test",
                ThirdPartyId = ThirdPartyId,
                Name = "Tested"
            };

            private static Mock<IMyCollectionRepository> myCollection;
            private static Album result;

            [ClassInitialize]
            public static async Task TestInitialize(TestContext _)
            {
                myCollection = new Mock<IMyCollectionRepository>();
                myCollection.Setup(c => c.FindBy3rdPartyId(ThirdPartyId)).ReturnsAsync(Album.CreateForUnknown(ThirdPartyId));

                var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
                thirdPartyMusicService.Setup(s => s.GetAlbumAsync(ThirdPartyId)).ReturnsAsync(ThirdPartyAlbum);

                myCollection.Setup(c => c.AddAsync(It.IsAny<Album>())).ReturnsAsync(ExpectedAddedAlbum);

                var logger = new Mock<ILogger<AlbumifyService>>().Object;
                var sut = new AlbumifyService(logger, thirdPartyMusicService.Object, myCollection.Object);
                result = await sut.AddAsync(ThirdPartyId);
            }

            [TestMethod]
            public void AddsTheAlbumToMyCollection() => myCollection.Verify(c => c.AddAsync(ThirdPartyAlbum), Times.Once);

            [TestMethod]
            public void ReturnsTheAddedAlbum() => result.Should().BeEquivalentTo(ExpectedAddedAlbum);

        }

        [TestClass]
        public class IfNotInMyCollection_AndNotInTheThirdPartyMusicService
        {
            private const string ThirdPartyId = "test-album-id";
            private static readonly Album UnknownAlbum = Album.CreateForUnknown(ThirdPartyId);

            private static Mock<IMyCollectionRepository> myCollection;
            private static Album result;

            [ClassInitialize]
            public static async Task ClassInitialize(TestContext _)
            {
                myCollection = new Mock<IMyCollectionRepository>();
                myCollection.Setup(c => c.FindBy3rdPartyId(It.IsAny<string>())).ReturnsAsync(Album.CreateForUnknown(ThirdPartyId));

                var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
                thirdPartyMusicService.Setup(s => s.GetAlbumAsync(It.IsAny<string>())).ReturnsAsync(UnknownAlbum);

                var logger = new Mock<ILogger<AlbumifyService>>().Object;
                var sut = new AlbumifyService(logger, thirdPartyMusicService.Object, myCollection.Object);
                result = await sut.AddAsync(ThirdPartyId);
            }

            [TestMethod]
            public void TheAlbumIsNotAdded() => myCollection.Verify(c => c.AddAsync(It.IsAny<Album>()), Times.Never);

            [TestMethod]
            public void ReturnsUnknownAlbum() => result.Should().BeEquivalentTo(UnknownAlbum);
        }
    }

    [TestClass]
    public class TheAlbumifyService_WhenGettingAnAlbum
    {
        private const string ThirdPartyId = "test-album-id";
        private readonly ILogger<AlbumifyService> logger = new Mock<ILogger<AlbumifyService>>().Object;

        private Mock<I3rdPartyMusicService> thirdPartyMusicService;
        private AlbumifyService sut;

        [TestInitialize]
        public void TestInitialize()
        {
            thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
            sut = new AlbumifyService(logger, thirdPartyMusicService.Object, null);
        }

        [TestMethod]
        public async Task GetsTheAlbumFromTheThirdPartyMusicService()
        {
            await sut.GetAsync(ThirdPartyId);
            thirdPartyMusicService.Verify(s => s.GetAlbumAsync(ThirdPartyId), Times.Once);
        }

        [TestMethod]
        public async Task ReturnsTheAlbum()
        {
            var expected = StubGettingAnAlbumFromThirdPartyMusicService();
            var result = await sut.GetAsync(ThirdPartyId);
            result.Should().BeEquivalentTo(expected);
        }

        private Album StubGettingAnAlbumFromThirdPartyMusicService()
        {
            var thirdPartyMusicServiceAlbum = new Album { ThirdPartyId = ThirdPartyId };
            thirdPartyMusicService.Setup(s => s.GetAlbumAsync(It.IsAny<string>())).ReturnsAsync(thirdPartyMusicServiceAlbum);
            return thirdPartyMusicServiceAlbum;
        }
    }
}