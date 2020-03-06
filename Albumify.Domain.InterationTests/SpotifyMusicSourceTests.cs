using Albumify.Domain.Spotify;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain.IntegrationTests
{
    [TestClass]
    public class TheSpotifyMusicSource
    {
        [TestClass]
        public class ThrowsAnException_WhenAuthenticatingUsingClientCredentialsFlow
        {
            [TestMethod]
            public async Task WithAnInvalidClientId()
            {
                var config = new ConfigurationBuilder()
                .AddUserSecrets("Albumify")
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "SpotifyClientId", "wrong" },
                })
                .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientSecret()
            {
                var config = new ConfigurationBuilder()
                    .AddUserSecrets("Albumify")
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "SpotifyClientSecret", "wrong" },
                    })
                    .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client secret. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            [TestMethod]
            public async Task WithAnInvalidClientIdAndInvalidClientSecret()
            {
                var config = new ConfigurationBuilder()
                    .AddUserSecrets("Albumify")
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "SpotifyClientId", "wrong" },
                        { "SpotifyClientSecret", "alsoWrong" }
                    })
                    .Build();

                var thrownEx = await TryToAuthenticate(config);

                Assert.AreEqual("Failed to authenticate with Spotify using Client Credentials Flow: Invalid client. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret",
                    thrownEx.Message);
            }

            private static async Task<Exception> TryToAuthenticate(IConfigurationRoot config)
            {
                Exception thrownEx = null;
                try
                {
                    var sut = new SpotifyMusicSource(config);
                    await sut.AuthenticateUsingClientCredentialsFlowAsync();

                }
                catch (Exception ex)
                {
                    thrownEx = ex;
                }

                Assert.IsNotNull(thrownEx);
                Assert.IsInstanceOfType(thrownEx, typeof(SpotifyAuthenticationException));
                return thrownEx;
            }
        }

        [TestClass]
        public class SuccessfullyAuthenticatesUsingClientCredentialsFlow
        {
            [TestMethod]
            public async Task WithValidCredentials()
            {
                var config = new ConfigurationBuilder().AddUserSecrets("Albumify").Build();
                var sut = new SpotifyMusicSource(config);
                var result = await sut.AuthenticateUsingClientCredentialsFlowAsync();
                Assert.AreEqual(3600, result.ExpiresIn);
                Assert.AreEqual("Bearer", result.TokenType);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
            }
        }
    }

    [TestClass]
    public class TheSpotifyMusicSource_WhenFindingAlbumsByArtist
    {
        // pagination
        // order is not supported by Spotify in v1
        // noAuthenticated  - 401

        [TestMethod]
        public async Task IsSuccessful_WithASingleWordArtistName()
        {
            var config = new ConfigurationBuilder().AddUserSecrets("Albumify").Build();
            var sut = new SpotifyMusicSource(config);
            var result = await sut.FindAlbumsByArtist("Jonezetta");

            var expected1 = new SpotifySearchAlbumResult
            {
                Name = "Popularity",
                NumberOfSongs = 11,
                ReleaseDate = new DateTime(2006, 1, 1)
                //album
            };
            var expected2 = new SpotifySearchAlbumResult
            {
                Name = "Cruel To Be Young",
                NumberOfSongs = 12,
                ReleaseDate = new DateTime(2008, 1, 1)
                //album
            };
            var expected3= new SpotifySearchAlbumResult
            {
                Name = "Sony Connect Set",
                NumberOfSongs = 5,
                ReleaseDate = new DateTime(2007, 1, 1)
                //single
            };
            var expected4 = new SpotifySearchAlbumResult
            {
                Name = "Three Songs",
                NumberOfSongs = 3,
                ReleaseDate = new DateTime(2006, 1, 1)
                //single
            };
            var expected = new List<SpotifySearchAlbumResult> { expected1, expected2, expected3, expected4 };
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [Description("also proves the keywords are matched in order")]
        public async Task IsSuccessful_WithAnArtistNameContainingSpaces()
        {
            var config = new ConfigurationBuilder().AddUserSecrets("Albumify").Build();
            var sut = new SpotifyMusicSource(config);
            var result = await sut.FindAlbumsByArtist("Search the City");

            var expected1 = new SpotifySearchAlbumResult
            {
                Name = "A Fire So Big The Heavens Can See It",
                NumberOfSongs = 10,
                ReleaseDate = new DateTime(2008, 1, 1)
                //album
            };
            var expected = new List<SpotifySearchAlbumResult> { expected1 };
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task ReturnsEmpty_WithAnUnknownArtist()
        {
            var config = new ConfigurationBuilder().AddUserSecrets("Albumify").Build();
            var sut = new SpotifyMusicSource(config);
            var result = await sut.FindAlbumsByArtist("Forever Changed");
            var expected = new List<SpotifySearchAlbumResult>();
            result.Should().BeEquivalentTo(expected);
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
}
