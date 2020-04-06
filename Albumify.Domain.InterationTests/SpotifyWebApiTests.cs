using Albumify.Domain.Models;
using Albumify.Domain.Spotify;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Albumify.Domain.IntegrationTests
{
    [TestClass]
    public class TheSpotifyWebApi_WhenFindingAlbumsByArtist
    {
        // pagination
        // noAuthenticated  - 401

        [TestClass]
        public class ReturnsAlbums
        {
            private static SpotifyWebApi sut;

            [ClassInitialize]
            public static void ClassInitialize(TestContext _)
            {
                var config = new TestingConfiguration().Build();
                sut = new SpotifyWebApi(new HttpClient(), new SpotifyClientCredentialsFlow(config, new HttpClient()));
            }

            [TestMethod]
            public async Task ForASingleWordArtistName()
            {
                var result = await sut.FindAlbumsByArtistAsync("Jonezetta");

                var expected1 = new SpotifySimplifiedAlbumObject
                {
                    Id = "3DYB0yIQYuOge2RjS7qHjs",
                    Name = "Popularity",
                    NumberOfSongs = 11,
                    ReleaseDate = "2006-01-01",
                    Type = "album",
                    Images = new List<SpotifyImageObject>
                {
                    new SpotifyImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                    new SpotifyImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                    new SpotifyImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
                }
                };
                var expected2 = new SpotifySimplifiedAlbumObject
                {
                    Id = "13pS0hN39dSGi9jqWpPmmB",
                    Name = "Cruel To Be Young",
                    NumberOfSongs = 12,
                    ReleaseDate = "2008-01-01",
                    Type = "album",
                    Images = new List<SpotifyImageObject>
                {
                    new SpotifyImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b2734ca3bc839b9589963f995100"},
                    new SpotifyImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e024ca3bc839b9589963f995100"},
                    new SpotifyImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d000048514ca3bc839b9589963f995100"}
                }
                };
                var expected3 = new SpotifySimplifiedAlbumObject
                {
                    Id = "1yuFbCBPcqxhI25V7tjtkV",
                    Name = "Sony Connect Set",
                    NumberOfSongs = 5,
                    ReleaseDate = "2007-01-01",
                    Type = "single",
                    Images = new List<SpotifyImageObject>
                {
                    new SpotifyImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b27357503c47d6c316e9f78e7915"},
                    new SpotifyImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e0257503c47d6c316e9f78e7915"},
                    new SpotifyImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d0000485157503c47d6c316e9f78e7915"}
                }
                };
                var expected4 = new SpotifySimplifiedAlbumObject
                {
                    Id = "3fbdu3A3yIYwmBNvik2vLk",
                    Name = "Three Songs",
                    NumberOfSongs = 3,
                    ReleaseDate = "2006-01-01",
                    Type = "single",
                    Images = new List<SpotifyImageObject>
                {
                    new SpotifyImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273cf66ce837b0b1ca468c2c16f"},
                    new SpotifyImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02cf66ce837b0b1ca468c2c16f"},
                    new SpotifyImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851cf66ce837b0b1ca468c2c16f"}
                }
                };
                var expected = new List<SpotifySimplifiedAlbumObject> { expected1, expected2, expected3, expected4 };
                result.Should().BeEquivalentTo(expected);
            }

            [TestMethod]
            [Description("also proves the keywords are matched in order")]
            public async Task ForAnArtistNameContainingSpaces()
            {
                var result = await sut.FindAlbumsByArtistAsync("Search the City");

                var expected1 = new SpotifySimplifiedAlbumObject
                {
                    Id = "2xmmuAkHQv3slZzaJwFGmB",
                    Name = "A Fire So Big The Heavens Can See It",
                    NumberOfSongs = 10,
                    ReleaseDate = "2008-01-01",
                    Type = "album"
                };
                var expected = new List<SpotifySimplifiedAlbumObject> { expected1 };
                result.Should().BeEquivalentTo(expected, o => o.Excluding(e => e.Images));
            }
        }

        [TestClass]
        public class ReturnsEmpty
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
            public async Task ForABlankArtist(string artist)
            {
                var result = await sut.FindAlbumsByArtistAsync(artist);
                result.Should().BeEmpty();
            }

            [TestMethod]
            public async Task ForAnUnknownArtist()
            {
                var result = await sut.FindAlbumsByArtistAsync("Forever Changed Again");
                result.Should().BeEmpty();
            }
        }

        /*
        {
  "albums": {
    "href": "https://api.spotify.com/v1/search?query=artist%3AJonezetta&type=album&offset=0&limit=20",
    "items": [
      {
        "album_type": "album",
        "artists": [
          {
            "external_urls": {
              "spotify": "https://open.spotify.com/artist/09l3QuYe7ExcyAZYosgVJx"
            },
            "href": "https://api.spotify.com/v1/artists/09l3QuYe7ExcyAZYosgVJx",
            "id": "09l3QuYe7ExcyAZYosgVJx",
            "name": "Jonezetta",
            "type": "artist",
            "uri": "spotify:artist:09l3QuYe7ExcyAZYosgVJx"
          }
        ],
        "available_markets": [
          "AD",
          "AE",
          "AR",
          "AT",
          "AU",
          "BE",
          "BG",
          "BH",
          "BO",
          "BR",
          "CA",
          "CH",
          "CL",
          "CO",
          "CR",
          "CY",
          "CZ",
          "DE",
          "DK",
          "DO",
          "DZ",
          "EC",
          "EE",
          "EG",
          "ES",
          "FI",
          "FR",
          "GB",
          "GR",
          "GT",
          "HK",
          "HN",
          "HU",
          "ID",
          "IE",
          "IL",
          "IN",
          "IS",
          "IT",
          "JO",
          "JP",
          "KW",
          "LB",
          "LI",
          "LT",
          "LU",
          "LV",
          "MA",
          "MC",
          "MT",
          "MX",
          "MY",
          "NI",
          "NL",
          "NO",
          "NZ",
          "OM",
          "PA",
          "PE",
          "PH",
          "PL",
          "PS",
          "PT",
          "PY",
          "QA",
          "RO",
          "SA",
          "SE",
          "SG",
          "SK",
          "SV",
          "TH",
          "TN",
          "TR",
          "TW",
          "US",
          "UY",
          "VN",
          "ZA"
        ],
        "external_urls": {
          "spotify": "https://open.spotify.com/album/3DYB0yIQYuOge2RjS7qHjs"
        },
        "href": "https://api.spotify.com/v1/albums/3DYB0yIQYuOge2RjS7qHjs",
        "id": "3DYB0yIQYuOge2RjS7qHjs",
        "images": [
          {
            "height": 640,
            "url": "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656",
            "width": 640
          },
          {
            "height": 300,
            "url": "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656",
            "width": 300
          },
          {
            "height": 64,
            "url": "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656",
            "width": 64
          }
        ],
        "name": "Popularity",
        "release_date": "2006-01-01",
        "release_date_precision": "day",
        "total_tracks": 11,
        "type": "album",
        "uri": "spotify:album:3DYB0yIQYuOge2RjS7qHjs"
      },
      {
        "album_type": "album",
        "artists": [
          {
            "external_urls": {
              "spotify": "https://open.spotify.com/artist/09l3QuYe7ExcyAZYosgVJx"
            },
            "href": "https://api.spotify.com/v1/artists/09l3QuYe7ExcyAZYosgVJx",
            "id": "09l3QuYe7ExcyAZYosgVJx",
            "name": "Jonezetta",
            "type": "artist",
            "uri": "spotify:artist:09l3QuYe7ExcyAZYosgVJx"
          }
        ],
        "available_markets": [
          "AD",
          "AE",
          "AR",
          "AT",
          "AU",
          "BE",
          "BG",
          "BH",
          "BO",
          "BR",
          "CA",
          "CH",
          "CL",
          "CO",
          "CR",
          "CY",
          "CZ",
          "DE",
          "DK",
          "DO",
          "DZ",
          "EC",
          "EE",
          "EG",
          "ES",
          "FI",
          "FR",
          "GB",
          "GR",
          "GT",
          "HK",
          "HN",
          "HU",
          "ID",
          "IE",
          "IL",
          "IN",
          "IS",
          "IT",
          "JO",
          "JP",
          "KW",
          "LB",
          "LI",
          "LT",
          "LU",
          "LV",
          "MA",
          "MC",
          "MT",
          "MX",
          "MY",
          "NI",
          "NL",
          "NO",
          "NZ",
          "OM",
          "PA",
          "PE",
          "PH",
          "PL",
          "PS",
          "PT",
          "PY",
          "QA",
          "RO",
          "SA",
          "SE",
          "SG",
          "SK",
          "SV",
          "TH",
          "TN",
          "TR",
          "TW",
          "US",
          "UY",
          "VN",
          "ZA"
        ],
        "external_urls": {
          "spotify": "https://open.spotify.com/album/13pS0hN39dSGi9jqWpPmmB"
        },
        "href": "https://api.spotify.com/v1/albums/13pS0hN39dSGi9jqWpPmmB",
        "id": "13pS0hN39dSGi9jqWpPmmB",
        "images": [
          {
            "height": 640,
            "url": "https://i.scdn.co/image/ab67616d0000b2734ca3bc839b9589963f995100",
            "width": 640
          },
          {
            "height": 300,
            "url": "https://i.scdn.co/image/ab67616d00001e024ca3bc839b9589963f995100",
            "width": 300
          },
          {
            "height": 64,
            "url": "https://i.scdn.co/image/ab67616d000048514ca3bc839b9589963f995100",
            "width": 64
          }
        ],
        "name": "Cruel To Be Young",
        "release_date": "2008-01-01",
        "release_date_precision": "day",
        "total_tracks": 12,
        "type": "album",
        "uri": "spotify:album:13pS0hN39dSGi9jqWpPmmB"
      },
      {
        "album_type": "single",
        "artists": [
          {
            "external_urls": {
              "spotify": "https://open.spotify.com/artist/09l3QuYe7ExcyAZYosgVJx"
            },
            "href": "https://api.spotify.com/v1/artists/09l3QuYe7ExcyAZYosgVJx",
            "id": "09l3QuYe7ExcyAZYosgVJx",
            "name": "Jonezetta",
            "type": "artist",
            "uri": "spotify:artist:09l3QuYe7ExcyAZYosgVJx"
          }
        ],
        "available_markets": [
          "AD",
          "AE",
          "AR",
          "AT",
          "AU",
          "BE",
          "BG",
          "BH",
          "BO",
          "BR",
          "CA",
          "CH",
          "CL",
          "CO",
          "CR",
          "CY",
          "CZ",
          "DE",
          "DK",
          "DO",
          "DZ",
          "EC",
          "EE",
          "EG",
          "ES",
          "FI",
          "FR",
          "GB",
          "GR",
          "GT",
          "HK",
          "HN",
          "HU",
          "ID",
          "IE",
          "IL",
          "IN",
          "IS",
          "IT",
          "JO",
          "KW",
          "LB",
          "LI",
          "LT",
          "LU",
          "LV",
          "MA",
          "MC",
          "MT",
          "MX",
          "MY",
          "NI",
          "NL",
          "NO",
          "NZ",
          "OM",
          "PA",
          "PE",
          "PH",
          "PL",
          "PS",
          "PT",
          "PY",
          "QA",
          "RO",
          "SA",
          "SE",
          "SG",
          "SK",
          "SV",
          "TH",
          "TN",
          "TR",
          "TW",
          "US",
          "UY",
          "VN",
          "ZA"
        ],
        "external_urls": {
          "spotify": "https://open.spotify.com/album/1yuFbCBPcqxhI25V7tjtkV"
        },
        "href": "https://api.spotify.com/v1/albums/1yuFbCBPcqxhI25V7tjtkV",
        "id": "1yuFbCBPcqxhI25V7tjtkV",
        "images": [
          {
            "height": 640,
            "url": "https://i.scdn.co/image/ab67616d0000b27357503c47d6c316e9f78e7915",
            "width": 640
          },
          {
            "height": 300,
            "url": "https://i.scdn.co/image/ab67616d00001e0257503c47d6c316e9f78e7915",
            "width": 300
          },
          {
            "height": 64,
            "url": "https://i.scdn.co/image/ab67616d0000485157503c47d6c316e9f78e7915",
            "width": 64
          }
        ],
        "name": "Sony Connect Set",
        "release_date": "2007-01-01",
        "release_date_precision": "day",
        "total_tracks": 5,
        "type": "album",
        "uri": "spotify:album:1yuFbCBPcqxhI25V7tjtkV"
      },
      {
        "album_type": "single",
        "artists": [
          {
            "external_urls": {
              "spotify": "https://open.spotify.com/artist/09l3QuYe7ExcyAZYosgVJx"
            },
            "href": "https://api.spotify.com/v1/artists/09l3QuYe7ExcyAZYosgVJx",
            "id": "09l3QuYe7ExcyAZYosgVJx",
            "name": "Jonezetta",
            "type": "artist",
            "uri": "spotify:artist:09l3QuYe7ExcyAZYosgVJx"
          }
        ],
        "available_markets": [
          "AD",
          "AE",
          "AR",
          "AT",
          "AU",
          "BE",
          "BG",
          "BH",
          "BO",
          "BR",
          "CA",
          "CH",
          "CL",
          "CO",
          "CR",
          "CY",
          "CZ",
          "DE",
          "DK",
          "DO",
          "DZ",
          "EC",
          "EE",
          "EG",
          "ES",
          "FI",
          "FR",
          "GB",
          "GR",
          "GT",
          "HK",
          "HN",
          "HU",
          "ID",
          "IE",
          "IL",
          "IN",
          "IS",
          "IT",
          "JO",
          "KW",
          "LB",
          "LI",
          "LT",
          "LU",
          "LV",
          "MA",
          "MC",
          "MT",
          "MX",
          "MY",
          "NI",
          "NL",
          "NO",
          "NZ",
          "OM",
          "PA",
          "PE",
          "PH",
          "PL",
          "PS",
          "PT",
          "PY",
          "QA",
          "RO",
          "SA",
          "SE",
          "SG",
          "SK",
          "SV",
          "TH",
          "TN",
          "TR",
          "TW",
          "US",
          "UY",
          "VN",
          "ZA"
        ],
        "external_urls": {
          "spotify": "https://open.spotify.com/album/3fbdu3A3yIYwmBNvik2vLk"
        },
        "href": "https://api.spotify.com/v1/albums/3fbdu3A3yIYwmBNvik2vLk",
        "id": "3fbdu3A3yIYwmBNvik2vLk",
        "images": [
          {
            "height": 640,
            "url": "https://i.scdn.co/image/ab67616d0000b273cf66ce837b0b1ca468c2c16f",
            "width": 640
          },
          {
            "height": 300,
            "url": "https://i.scdn.co/image/ab67616d00001e02cf66ce837b0b1ca468c2c16f",
            "width": 300
          },
          {
            "height": 64,
            "url": "https://i.scdn.co/image/ab67616d00004851cf66ce837b0b1ca468c2c16f",
            "width": 64
          }
        ],
        "name": "Three Songs",
        "release_date": "2006-01-01",
        "release_date_precision": "day",
        "total_tracks": 3,
        "type": "album",
        "uri": "spotify:album:3fbdu3A3yIYwmBNvik2vLk"
      }
    ],
    "limit": 20,
    "next": null,
    "offset": 0,
    "previous": null,
    "total": 4
  }
}
         */
    }

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
}
