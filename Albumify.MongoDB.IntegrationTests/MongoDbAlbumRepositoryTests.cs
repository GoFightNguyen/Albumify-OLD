using Albumify.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Albumify.MongoDB.IntegrationTests
{
    [TestClass]
    public class TheMongoDbAlbumRepository_WhenAddingAnAlbum
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
                    var sut = new MongoDbAlbumRepository(config);
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

            private static MongoDbAlbumRepository _sut;
            private static Album _returnedAlbum;

            [ClassInitialize]
            public static async Task ClassInitialize(TestContext _)
            {
                _sut = new MongoDbAlbumRepository(new TestingConfiguration().Build());
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
    public class TheMongoDbAlbumRepository_FindBy3rdPartyId
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

        private static MongoDbAlbumRepository _sut;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext _)
        {
            _sut = new MongoDbAlbumRepository(new TestingConfiguration().Build());
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
            var result = await _sut.FindBy3rdPartyId("UnknownThirdPartyId");
            result.Should().BeEquivalentTo(Album.CreateForUnknown("UnknownThirdPartyId"));
        }

        [TestMethod]
        public async Task ReturnsMatchingAlbum()
        {
            var result = await _sut.FindBy3rdPartyId(ThirdPartyId);
            result.Should().BeEquivalentTo(album);
        }
    }
}
