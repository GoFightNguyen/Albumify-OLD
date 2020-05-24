using Albumify.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Albumify.MongoDB.IntegrationTests
{
    [TestClass]
    public class TheMyCollectionInMongoDB_WhenAddingAnAlbum
    {
        [TestClass]
        public class ThrowsAnException
        {
            [TestMethod]
            public async Task ForAnInvalidUsername()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbUsername()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Message.Should().Be("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBUsername and MongoDBPassword");
                thrownEx.InnerException.Should().BeOfType<MongoAuthenticationException>();
            }

            [TestMethod]
            public async Task ForAnInvalidPassword()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbPassword()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Message.Should().Be("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBUsername and MongoDBPassword");
                thrownEx.InnerException.Should().BeOfType<MongoAuthenticationException>();
            }

            [TestMethod]
            public async Task ForAnInvalidHost()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbHost()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Should().NotBeNull();
                thrownEx.Message.Should().Be("Failed to connect to MongoDB because of a timeout. Please verify the configuration for MongoDBHost");
                thrownEx.InnerException.Should().BeOfType<TimeoutException>();
            }

            [TestMethod]
            public async Task ForAnInvalidHostScheme()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbHostScheme()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Should().NotBeNull();
                thrownEx.Message.Should().Be("Failed to connect to MongoDB. Please verify the configuration for MongoDBHostScheme");
                thrownEx.InnerException.Should().BeOfType<MongoConfigurationException>();
            }

            private static async Task<Exception> TryToAddAlbum(IConfiguration config)
            {
                Exception thrownEx = null;
                try
                {
                    var sut = new MyCollectionInMongoDB(config);
                    await sut.AddAsync(new Album());
                }
                catch (Exception ex)
                {
                    thrownEx = ex;
                }

                Assert.IsNotNull(thrownEx);
                Assert.IsInstanceOfType(thrownEx, typeof(AlbumRepositoryException));
                return thrownEx;
            }
        }

        [TestClass]
        public class AndSuccessful
        {
            private static readonly Album album = new Album
            {
                Name = "Popularity",
                Label = "Tooth & Nail (TNN)",
                ReleaseDate = "2006-01-01",
                Type = "album",
                ThirdPartyId = "3DYB0yIQYuOge2RjS7qHjs"
            };

            private static MyCollectionInMongoDB _sut;
            private static Album _returnedAlbum;

            [ClassInitialize]
            public static async Task ClassInitialize(TestContext _)
            {
                _sut = new MyCollectionInMongoDB(new TestingConfiguration().Build());
                _returnedAlbum = await _sut.AddAsync(album);
            }

            [ClassCleanup]
            public static async Task ClassCleanup()
            {
                await _sut.RemoveAsync(album.Id);
            }

            [TestMethod]
            public void UpdatesTheIdOnTheAlbumPassedIn() => album.Id.Should().NotBeNullOrWhiteSpace();

            [TestMethod]
            public void ReturnsTheInsertedAlbum() => _returnedAlbum.Should().BeEquivalentTo(album);

            [TestMethod]
            public async Task TheAlbumIsInMongoDB()
            {
                var createdAlbum = await _sut.GetAsync(album.Id);
                createdAlbum.Should().BeEquivalentTo(album);
            }
        }
    }

    [TestClass]
    public class TheMyCollectionInMongoDB_FindBy3rdPartyId
    {
        private const string ThirdPartyId = "3DYB0yIQYuOge2RjS7qHjs";
        private static readonly Album album = new Album
        {
            Name = "Popularity",
            Label = "Tooth & Nail (TNN)",
            ReleaseDate = "2006-01-01",
            Type = "album",
            ThirdPartyId = ThirdPartyId
        };

        private static MyCollectionInMongoDB _sut;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _sut = new MyCollectionInMongoDB(new TestingConfiguration().Build());
            await _sut.AddAsync(album);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await _sut.RemoveAsync(album.Id);
        }

        [TestMethod]
        public async Task ReturnsUnknownAlbum_WhenNotFound()
        {
            var result = await _sut.FindBy3rdPartyIdAsync("UnknownThirdPartyId");
            result.Should().BeEquivalentTo(Album.CreateForUnknown("UnknownThirdPartyId"));
        }

        [TestMethod]
        public async Task ReturnsMatchingAlbum()
        {
            var result = await _sut.FindBy3rdPartyIdAsync(ThirdPartyId);
            result.Should().BeEquivalentTo(album);
        }
    }

    [TestClass]
    public class TheMyCollectionInMongoDB_WhenDeterminingWhichArtistsAreInMycollection
    {
        private static readonly Artist artistHasAnAlbumAndPartOfAMultiArtistAlbum = new Artist { ThirdPartyId = "artist1" };
        private static readonly Artist artistOnlyInAMultiArtistAlbum = new Artist { ThirdPartyId = "artist2" };
        private static readonly Artist onlyArtistInMultipleAlbums = new Artist { ThirdPartyId = "artist3" };

        private static readonly Album album1 = new Album { Id = "whichArtistsAreInCollection_album1", Artists = new List<Artist> { artistHasAnAlbumAndPartOfAMultiArtistAlbum } };
        private static readonly Album album2 = new Album { Id = "whichArtistsAreInCollection_album2", Artists = new List<Artist> { artistHasAnAlbumAndPartOfAMultiArtistAlbum, artistOnlyInAMultiArtistAlbum } };
        private static readonly Album album3 = new Album { Id = "whichArtistsAreInCollection_album3", Artists = new List<Artist> { onlyArtistInMultipleAlbums } };
        private static readonly Album album4 = new Album { Id = "whichArtistsAreInCollection_album4", Artists = new List<Artist> { new Artist { ThirdPartyId = "another artist" } } };
        private static readonly Album album5 = new Album { Id = "whichArtistsAreInCollection_album5", Artists = new List<Artist> { onlyArtistInMultipleAlbums } };

        private static MyCollectionInMongoDB _sut;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _sut = new MyCollectionInMongoDB(new TestingConfiguration().Build());
            await _sut.AddAsync(album1);
            await _sut.AddAsync(album2);
            await _sut.AddAsync(album3);
            await _sut.AddAsync(album4);
            await _sut.AddAsync(album5);
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await _sut.RemoveAsync(album1.Id);
            await _sut.RemoveAsync(album2.Id);
            await _sut.RemoveAsync(album3.Id);
            await _sut.RemoveAsync(album4.Id);
            await _sut.RemoveAsync(album5.Id);
        }

        [TestMethod]
        public async Task ReturnsTheThirdPartyId_IfTheArtistIsOneOfMultipleForASingleAlbum()
        {
            var result = await _sut.ContainsWhichOfTheseArtists(artistOnlyInAMultiArtistAlbum.ThirdPartyId);
            result.Single().Should().Be(artistOnlyInAMultiArtistAlbum.ThirdPartyId);
        }

        [TestMethod]
        public async Task ReturnsTheThirdPartyId_IfTheArtistHasOwnAlbums()
        {
            var result = await _sut.ContainsWhichOfTheseArtists(onlyArtistInMultipleAlbums.ThirdPartyId);
            result.Single().Should().Be(onlyArtistInMultipleAlbums.ThirdPartyId);
        }

        [TestMethod]
        public async Task ReturnsTheThirdPartyId_IfTheArtistHasOwnAlbumAndIsOneOfMultipleForASingleAlbum()
        {
            var result = await _sut.ContainsWhichOfTheseArtists(artistHasAnAlbumAndPartOfAMultiArtistAlbum.ThirdPartyId);
            result.Single().Should().Be(artistHasAnAlbumAndPartOfAMultiArtistAlbum.ThirdPartyId);
        }

        [TestMethod]
        public async Task ReturnsEmpty_IfTheArtistIsNotPartOfAnyAlbums()
        {
            var result = await _sut.ContainsWhichOfTheseArtists("noContributionToAnyAlbum");
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ReturnsMultipleThirdPartyIds_IfMultipleOfTheSpecifiedArtistsHaveAlbums()
        {
            var result = await _sut.ContainsWhichOfTheseArtists(
                artistHasAnAlbumAndPartOfAMultiArtistAlbum.ThirdPartyId,
                "noContributionToAnyAlbum",
                artistOnlyInAMultiArtistAlbum.ThirdPartyId,
                onlyArtistInMultipleAlbums.ThirdPartyId
                );
            result.Should().BeEquivalentTo(new List<string>{
                artistHasAnAlbumAndPartOfAMultiArtistAlbum.ThirdPartyId,
                artistOnlyInAMultiArtistAlbum.ThirdPartyId,
                onlyArtistInMultipleAlbums.ThirdPartyId });
        }
    }
}
