using Albumify.Domain;
using Albumify.Domain.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Albumify.Spotify.IntegrationTests
{
    [TestClass]
    public class TheSpotifyWebApi_WhenGettingASpecificAlbum
    {
        private static I3rdPartyMusicService sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            var config = new TestingConfiguration().Build();
            sut = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
        }

        [TestMethod]
        public async Task ReturnsTheFoundAlbum()
        {
            var result = await sut.GetAlbumAsync("3DYB0yIQYuOge2RjS7qHjs");
            var expected = new Album
            {
                Artists = new List<Artist>
                {
                    new Artist { ThirdPartyId = "09l3QuYe7ExcyAZYosgVJx", Name = "Jonezetta" }
                },
                ThirdPartyId = "3DYB0yIQYuOge2RjS7qHjs",
                Images = new List<Image>
                {
                    new Image {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                    new Image {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                    new Image {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
                },
                Label = "Tooth & Nail (TNN)",
                Name = "Popularity",
                ReleaseDate = "2006-01-01",
                Tracks = new List<Track>
                {
                    new Track { Name = "Welcome Home", Number = 1},
                    new Track { Name = "Get Ready (Hot Machete)", Number = 2},
                    new Track { Name = "Communicate", Number = 3},
                    new Track { Name = "Man In A 3K Suit", Number = 4},
                    new Track { Name = "Backstabber", Number = 5},
                    new Track { Name = "Popularity", Number = 6},
                    new Track { Name = "The Love That Carries Me", Number = 7},
                    new Track { Name = "The City We Live In", Number = 8},
                    new Track { Name = "Bringin' It Back Tonight... Everybody Start", Number = 9},
                    new Track { Name = "Burn It Down!", Number = 10},
                    new Track { Name = "Imagination", Number = 11}
                },
                Type = "album"
            };

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task ReturnsUnknownAlbum_ForNonExistingId()
        {
            var result = await sut.GetAlbumAsync("NonExistingIdIsWhatIAm");
            var expected = new Album
            {
                Artists = new List<Artist> { new Artist { Name = "Unknown Artist", ThirdPartyId = "Unknown" } },
                Id = Album.UnknownAlbumId,
                Images = new List<Image>(),
                Label = "",
                Name = "Unknown Album",
                ReleaseDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
                ThirdPartyId = "NonExistingIdIsWhatIAm",
                Tracks = new List<Track>(),
                Type = "album"
            };
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task ReturnsUnknownAlbum_ForInvalidId()
        {
            var result = await sut.GetAlbumAsync("InvalidIdTooShort");
            var expected = new Album
            {
                Artists = new List<Artist> { new Artist { Name = "Unknown Artist", ThirdPartyId = "Unknown" } },
                Id = Album.UnknownAlbumId,
                Images = new List<Image>(),
                Label = "",
                Name = "Unknown Album",
                ReleaseDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
                ThirdPartyId = "InvalidIdTooShort",
                Tracks = new List<Track>(),
                Type = "album"
            };
            result.Should().BeEquivalentTo(expected);
        }
    }

    [TestClass]
    public class TheSpotifyWebApi_WhenSearchingArtistsByName
    {
        private static SpotifyWebApi sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            var config = new TestingConfiguration().Build();
            sut = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public async Task ReturnsEmpty_ForABlankArtist(string artistName)
        {
            var result = await sut.SearchArtistsByNameAsync(artistName);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ReturnsEmpty_IfThereAreNoMatches()
        {
            const string ArtistName = "Forever Changed Again";
            var result = await sut.SearchArtistsByNameAsync(ArtistName);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ReturnsTheArtists()
        {
            const string ArtistName = "Norma Jean";
            var expected = await BuildExpectedAsync();
            var result = await sut.SearchArtistsByNameAsync(ArtistName);
            result.Should().BeEquivalentTo(expected);
        }

        private static async Task<IEnumerable<Artist>> BuildExpectedAsync()
        {
            var json = await File.ReadAllTextAsync("SearchArtistsByName-Items-NormaJean.json");
            var artists = JsonSerializer.Deserialize<TestHelpingObject>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            return artists.Items;
        }
        private class TestHelpingObject
        {
            public List<Artist> Items { get; set; }
        }
    }

    [TestClass]
    public class TheSpotifyWebApi_WhenGettingAnArtistsAlbums
    {
        [TestMethod]
        public async Task ReturnsTheAlbums()
        {
            const string THIRD_PARTY_ID_ARTIST = "09l3QuYe7ExcyAZYosgVJx";
            var config = new TestingConfiguration().Build();
            var sut = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
            var expected = await BuildExpectedAsync();
            var result = await sut.GetAnArtistsAlbumsAsync(THIRD_PARTY_ID_ARTIST);
            result.Should().BeEquivalentTo(expected);
        }

        private static async Task<IEnumerable<Album>> BuildExpectedAsync()
        {
            var json = await File.ReadAllTextAsync("GetAnArtistsAlbums-Jonezetta.json");
            var albums = JsonSerializer.Deserialize<TestHelpingObject>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            return albums.Items;
        }
        private class TestHelpingObject
        {
            public List<Album> Items { get; set; }
        }
    }
}
