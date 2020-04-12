using Albumify.Domain;
using Albumify.Domain.Models;
using Albumify.MongoDB;
using Albumify.Spotify;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Albumify.AcceptanceTests
{
    [TestClass]
    public class TheAlbumifyService_WhenAddingAnAlbum
    {
        [TestClass]
        public class NotInMyCollection
        {
            private const string ThirdPartyId = "1oDkUnjCBAHsaQtr0J0s3t";

            private static MongoDbAlbumRepository _myCollectionInMongoDB;
            private static AlbumifyService _sut;
            private static Album _result;

            private static readonly Album _expected = new Album
            {
                Artists = new List<Artist>
                    {
                        new Artist{ Name = "Lecrae", ThirdPartyId = "1CFCsEqKrCyvAFKOATQHiW" },
                        new Artist{ Name = "Zaytoven", ThirdPartyId = "1mceaxtjWdEmwoDVAlkC41" }
                    },
                Images = new List<Image>
                    {
                        new Image { Height = 640, Url = "https://i.scdn.co/image/ab67616d0000b2739d4ee0a18ce6768c68e90df4", Width = 640 },
                        new Image { Height = 300, Url = "https://i.scdn.co/image/ab67616d00001e029d4ee0a18ce6768c68e90df4", Width = 300 },
                        new Image { Height = 64, Url = "https://i.scdn.co/image/ab67616d000048519d4ee0a18ce6768c68e90df4", Width = 64 }
                    },
                Label = "Reach Records",
                Name = "Let the Trap Say Amen",
                ReleaseDate = "2018-06-22",
                ThirdPartyId = ThirdPartyId,
                Tracks = new List<Track>
                    {
                        new Track { Name = "Get Back Right", Number = 1 },
                        new Track { Name = "Preach", Number = 2 },
                        new Track { Name = "2 Sides Of The Game", Number = 3 },
                        new Track { Name = "Plugged In", Number = 4 },
                        new Track { Name = "Holy Water", Number = 5 },
                        new Track { Name = "Blue Strips", Number = 6 },
                        new Track { Name = "Only God Can Judge Me", Number = 7 },
                        new Track { Name = "Yet", Number = 8 },
                        new Track { Name = "I Can't Lose", Number = 9 },
                        new Track { Name = "Switch", Number = 10 },
                        new Track { Name = "Can't Block It", Number = 11 },
                        new Track { Name = "Fly Away", Number = 12 },
                        new Track { Name = "By Chance", Number = 13 }
                    },
                Type = "album"
            };

            [ClassInitialize]
            public static async Task ClassInitialize(TestContext _)
            {
                var logger = new NullLogger<AlbumifyService>();
                var config = new TestingConfiguration().Build();
                var spotifyMusicService = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
                _myCollectionInMongoDB = new MongoDbAlbumRepository(config);
                _sut = new AlbumifyService(logger, spotifyMusicService, _myCollectionInMongoDB);

                _result = await _sut.AddAsync(ThirdPartyId);
                _expected.Id = _result.Id;
            }

            [ClassCleanup]
            public static async Task ClassCleanup() => await _myCollectionInMongoDB.RemoveAsync(_result.Id);

            [TestMethod]
            public async Task AddsTheAlbum()
            {
                var albumFromCollection = await _sut.GetAsync(ThirdPartyId);
                albumFromCollection.Should().BeEquivalentTo(_expected);
            }

            [TestMethod]
            public void ReturnsTheAlbum() => _result.Should().BeEquivalentTo(_expected);
        }

        [TestClass]
        public class AlreadyInMyCollection
        {
            private const string ThirdPartyId = "5DPZqC3ySZkJClCvZlIq6K";

            private static MongoDbAlbumRepository _myCollectionInMongoDB;
            private static AlbumifyService _sut;
            private static Album _result;

            private static readonly Album _expected = new Album
            {
                Artists = new List<Artist>
                    {
                        new Artist{ Name = "Lecrae", ThirdPartyId = "1CFCsEqKrCyvAFKOATQHiW" }
                    },
                Images = new List<Image>
                    {
                        new Image { Height = 640, Url = "https://i.scdn.co/image/ab67616d0000b273b138ac1dc14bc1ee426e9383", Width = 640 },
                        new Image { Height = 300, Url = "https://i.scdn.co/image/ab67616d00001e02b138ac1dc14bc1ee426e9383", Width = 300 },
                        new Image { Height = 64, Url = "https://i.scdn.co/image/ab67616d00004851b138ac1dc14bc1ee426e9383", Width = 64 }
                    },
                Label = "Reach Records/Columbia",
                Name = "All Things Work Together",
                ReleaseDate = "2017-09-22",
                ThirdPartyId = ThirdPartyId,
                Tracks = new List<Track>
                    {
                        new Track { Name = "Always Knew", Number = 1 },
                        new Track { Name = "Facts", Number = 2 },
                        new Track { Name = "Broke", Number = 3 },
                        new Track { Name = "Blessings", Number = 4 },
                        new Track { Name = "Whatchu Mean", Number = 5 },
                        new Track { Name = "Hammer Time", Number = 6 },
                        new Track { Name = "Come and Get Me", Number = 7 },
                        new Track { Name = "Lucked Up", Number = 8 },
                        new Track { Name = "Wish You the Best", Number = 9 },
                        new Track { Name = "Can't Stop Me Now (Destination)", Number = 10 },
                        new Track { Name = "I'll Find You", Number = 11 },
                        new Track { Name = "8:28", Number = 12 },
                        new Track { Name = "Cry For You", Number = 13 },
                        new Track { Name = "Worth It", Number = 14 }
                    },
                Type = "album"
            };

            [ClassInitialize]
            public static async Task ClassInitialize(TestContext _)
            {
                var logger = new NullLogger<AlbumifyService>();
                var config = new TestingConfiguration().Build();
                var spotifyMusicService = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
                _myCollectionInMongoDB = new MongoDbAlbumRepository(config);
                _sut = new AlbumifyService(logger, spotifyMusicService, _myCollectionInMongoDB);

                var originalAdd = await _sut.AddAsync(ThirdPartyId);
                _expected.Id = originalAdd.Id;

                _result = await _sut.AddAsync(ThirdPartyId);
            }

            [ClassCleanup]
            public static async Task ClassCleanup() => await _myCollectionInMongoDB.RemoveAsync(_expected.Id);

            [TestMethod]
            public async Task DoesNotAddTheAlbumAgain()
            {
                var count = await _myCollectionInMongoDB._albums.CountDocumentsAsync(a => a.ThirdPartyId == ThirdPartyId);
                count.Should().Be(1);
            }

            [TestMethod]
            public void ReturnsTheAlbum() => _result.Should().BeEquivalentTo(_expected);
        }
    }
}
