using Albumify.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Albumify.Domain.IntegrationTests
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

                thrownEx.Message.Should().Be("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBHost, MongoDBUsername, and MongoDBPassword");
                thrownEx.InnerException.Should().BeOfType<MongoDB.Driver.MongoAuthenticationException>();
            }

            [TestMethod]
            public async Task ForAnInvalidPassword()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbPassword()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Message.Should().Be("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBHost, MongoDBUsername, and MongoDBPassword");
                thrownEx.InnerException.Should().BeOfType<MongoDB.Driver.MongoAuthenticationException>();
            }

            [TestMethod]
            public async Task ForAnInvalidHost()
            {
                var config = new TestingConfiguration()
                    .WithWrongMongoDbHost()
                    .Build();
                var thrownEx = await TryToAddAlbum(config);

                thrownEx.Should().NotBeNull();
                thrownEx.Message.Should().Be("Failed to connect to MongoDB because of a timeout. Please verify the configuration for MongoDBHost, MongoDBUsername, and MongoDBPassword");
                thrownEx.InnerException.Should().BeOfType<TimeoutException>();
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

        [TestMethod]
        public async Task SuccessfullyAdds_TheAlbum()
        {
            // Arrange
            var album = new Album
            {
                Name = "Popularity",
                Label = "Tooth & Nail (TNN)",
                ReleaseDate = "2006-01-01",
                Type = "album",
                NumberOfSongs = 11,
                SpotifyId = "3DYB0yIQYuOge2RjS7qHjs"
            };

            // Act            
            var sut = new MongoDbAlbumRepository(new TestingConfiguration().Build());
            await sut.AddAsync(album);
            var createdAlbum = await sut.GetAsync(album.Id);

            // Assert
            createdAlbum.Should().BeEquivalentTo(album);

            // Cleanup
            await sut.RemoveAsync(album.Id);
        }
    }
}
