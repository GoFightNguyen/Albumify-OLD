using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Albumify.Domain.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;

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
                var logger = new NullLogger<AlbumifyService>();
                myCollection = new Mock<IMyCollectionRepository>();
                thirdPartyMusicService = new Mock<I3rdPartyMusicService>();

                myCollection.Setup(m => m.FindBy3rdPartyIdAsync(ThirdPartyId)).ReturnsAsync(CollectionAlbum);

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
                myCollection.Setup(c => c.FindBy3rdPartyIdAsync(ThirdPartyId)).ReturnsAsync(Album.CreateForUnknown(ThirdPartyId));

                var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
                thirdPartyMusicService.Setup(s => s.GetAlbumAsync(ThirdPartyId)).ReturnsAsync(ThirdPartyAlbum);

                myCollection.Setup(c => c.AddAsync(It.IsAny<Album>())).ReturnsAsync(ExpectedAddedAlbum);

                var logger = new NullLogger<AlbumifyService>();
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
                myCollection.Setup(c => c.FindBy3rdPartyIdAsync(It.IsAny<string>())).ReturnsAsync(Album.CreateForUnknown(ThirdPartyId));

                var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
                thirdPartyMusicService.Setup(s => s.GetAlbumAsync(It.IsAny<string>())).ReturnsAsync(UnknownAlbum);

                var logger = new NullLogger<AlbumifyService>();
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
        [TestClass]
        public class IfInMyCollection
        {
            private const string ThirdPartyId = "test-album-id";
            private static readonly Album CollectionAlbum = new Album
            {
                Id = "123-456-789",
                Label = "Unit Test",
                ThirdPartyId = ThirdPartyId
            };

            private static Mock<I3rdPartyMusicService> ThirdPartyMusicService;
            private static Album Result;

            [ClassInitialize]
            public static async Task ClassIniitalize(TestContext _)
            {
                var logger = new NullLogger<AlbumifyService>();
                var myCollection = new Mock<IMyCollectionRepository>();
                ThirdPartyMusicService = new Mock<I3rdPartyMusicService>();

                myCollection.Setup(m => m.FindBy3rdPartyIdAsync(ThirdPartyId)).ReturnsAsync(CollectionAlbum);

                var sut = new AlbumifyService(logger, ThirdPartyMusicService.Object, myCollection.Object);
                Result = await sut.GetAsync(ThirdPartyId);
            }

            [TestMethod]
            public void DoesNotQuery3rdPartyMusicService() => ThirdPartyMusicService.Verify(s => s.GetAlbumAsync(It.IsAny<string>()), Times.Never);

            [TestMethod]
            public void ReturnsTheAlbumFromMyCollection() => Result.Should().BeEquivalentTo(CollectionAlbum);
        }

        [TestClass]
        public class IfNotInMyCollection
        {
            private const string ThirdPartyId = "test-album-id";

            private Mock<I3rdPartyMusicService> _thirdPartyMusicService;
            private AlbumifyService _sut;

            [TestInitialize]
            public void TestInitialize()
            {
                var logger = new NullLogger<AlbumifyService>();
                var myCollection = new Mock<IMyCollectionRepository>();
                myCollection.Setup(c => c.FindBy3rdPartyIdAsync(It.IsAny<string>())).ReturnsAsync(Album.CreateForUnknown(ThirdPartyId));

                _thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
                _sut = new AlbumifyService(logger, _thirdPartyMusicService.Object, myCollection.Object);
            }

            [TestMethod]
            public async Task ButInThirdPartyMusicService_ReturnsTheAlbum()
            {
                var album = StubThirdPartyMusicServiceToReturnAlbum();
                var result = await _sut.GetAsync(ThirdPartyId);
                result.Should().BeEquivalentTo(album);
            }

            [TestMethod]
            public async Task AndNotInThirdPartyMusicService_ReturnsUnknownAlbum()
            {
                var unknownAlbum = StubThirdPartyMusicServiceToReturnUnknownAlbum();
                var result = await _sut.GetAsync(ThirdPartyId);
                result.Should().BeEquivalentTo(unknownAlbum);
            }

            private Album StubThirdPartyMusicServiceToReturnAlbum()
            {
                var thirdPartyAlbum = new Album
                {
                    Label = "Unit Test",
                    ThirdPartyId = ThirdPartyId,
                    Name = "Tested"
                };

                _thirdPartyMusicService.Setup(s => s.GetAlbumAsync(ThirdPartyId)).ReturnsAsync(thirdPartyAlbum);
                return thirdPartyAlbum;
            }

            private Album StubThirdPartyMusicServiceToReturnUnknownAlbum()
            {
                var unknownAlbum = Album.CreateForUnknown(ThirdPartyId);
                _thirdPartyMusicService.Setup(s => s.GetAlbumAsync(It.IsAny<string>())).ReturnsAsync(unknownAlbum);
                return unknownAlbum;
            }
        }
    }

    [TestClass]
    public class TheAlbumifyService_WhenSearchingArtistsByName
    {
        [TestMethod]
        public async Task ReturnsTheResultsOfTheThirdPartyMusicService()
        {
            // Arrange
            const string ArtistName = "Norma Jean";
            var expected = new List<Artist>
            {
                new Artist
                {
                    Name = "Norma Jean Martine",
                    ThirdPartyId = "2fsk4VlJdNF6G8cCMDrrzB",
                    Images = new List<Image>
                    {
                        new Image
                        {
                            Height = 640,
                            Url = "https://i.scdn.co/image/3bafc579f8582e3d65eb35e31f1891901aa76749",
                            Width = 640
                        },
                        new Image
                        {
                            Height = 320,
                            Url = "https://i.scdn.co/image/2ea1b920b93a7d1480009135ec2bc15671725154",
                            Width = 320
                        },
                        new Image
                        {
                            Height = 160,
                            Url = "https://i.scdn.co/image/037893c02cb3e449c2f97ef8e71a26889f6cdec8",
                            Width = 160
                        }
                    }
                },
                new Artist
                {
                    Name = "Norma Jean",
                    ThirdPartyId = "55b0Gfm53udtGBs8mmNXrH",
                    Images = new List<Image>
                    {
                        new Image
                        {
                            Height = 640,
                            Url = "https://i.scdn.co/image/4f4fb36a3c564d0078be2e21962ee47724747b44",
                            Width = 640
                        },
                        new Image
                        {
                            Height = 320,
                            Url = "https://i.scdn.co/image/8717743daaf2e2b2f53008e5835232f9e90d5897",
                            Width = 320
                        },
                        new Image
                        {
                            Height = 160,
                            Url = "https://i.scdn.co/image/e99dfcc008ac74efced891cda956f6e4d0463260",
                            Width = 160
                        }
                    }
                },
                new Artist
                {
                    Name = "Norma Jean Moslander",
                    ThirdPartyId = "4ZhOkZoqfppPsyRFuoVgTv"
                }
            };

            var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
            thirdPartyMusicService.Setup(s => s.SearchArtistsByNameAsync(ArtistName)).ReturnsAsync(expected);

            // Act
            var logger = new NullLogger<AlbumifyService>();
            var sut = new AlbumifyService(logger, thirdPartyMusicService.Object, null);
            var result = await sut.SearchArtistsByNameAsync(ArtistName);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    [TestClass]
    public class TheAlbumifyService_WhenGettingAnArtistsAlbums
    {
        [TestMethod]
        public async Task ReturnsTheResultsOfTheThirdPartyMusicService()
        {
            // Arrange
            const string THIRD_PARTY_ID_ARTIST = "55b0Gfm53udtGBs8mmNXrH";
            var artist = new Artist { ThirdPartyId = THIRD_PARTY_ID_ARTIST, Name = "Norma Jean" };
            var expected1 = new Album
            {
                ThirdPartyId = "0DYeH7SR1w2dvQ05eR0dIP",
                Name = "All Hail",
                ReleaseDate = "2019-10-25",
                Type = "album",
                Artists = new List<Artist> { artist }
            };
            var expected2 = new Album
            {
                ThirdPartyId = "1eEbsiMBr41CdwD8tGSamK",
                Name = "Polar Similar",
                ReleaseDate = "2016-09-09",
                Type = "album",
                Artists = new List<Artist> { artist }
            };
            var expected = new List<Album> { expected1, expected2 };

            var thirdPartyMusicService = new Mock<I3rdPartyMusicService>();
            thirdPartyMusicService.Setup(s => s.GetAnArtistsAlbumsAsync(THIRD_PARTY_ID_ARTIST)).ReturnsAsync(expected);

            // Act
            var logger = new NullLogger<AlbumifyService>();
            var sut = new AlbumifyService(logger, thirdPartyMusicService.Object, null);
            var result = await sut.GetAnArtistsAlbumsAsync(THIRD_PARTY_ID_ARTIST);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}