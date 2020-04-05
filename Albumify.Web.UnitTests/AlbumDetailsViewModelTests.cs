using Albumify.Domain.Models;
using Albumify.Web.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albumify.Web.UnitTests
{
    [TestClass]
    public class TheAlbumDetailsViewModel_WhenConvertingFromASpotifyAlbumObject
    {
        public static Album CreateDomainAlbum()
        {
            return new Album
            {
                ThirdPartyId = "3DYB0yIQYuOge2RjS7qHjs",
                Label = "Albumify Tests",
                Name = "Album",
                ReleaseDate = "2019-03-18",
                Artists = new List<Artist>
                {
                    new Artist { ThirdPartyId = "09l3QuYe7ExcyAZYosgVJx", Name = "Jonezetta" },
                    new Artist { ThirdPartyId = "MadeUp", Name = "Made Up" },
                }
            };
        }

        [TestMethod]
        public void CopiesFirstArtist()
        {
            var source = CreateDomainAlbum();
            var result = new AlbumDetailsViewModel(source);
            var expected = new ArtistViewModel { SpotifyId = "09l3QuYe7ExcyAZYosgVJx", Name = "Jonezetta" };
            result.Artist.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ConvertsImages()
        {
            var source = CreateDomainAlbum();
            source.Images = new List<Image>
            {
                new Image {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                new Image {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                new Image {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
            };
            var result = new AlbumDetailsViewModel(source);
            var expected = new List<ImageViewModel>
            {
                new ImageViewModel {Height = 640, Width = 640, Path = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 64, Width = 64, Path = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 300, Width = 300, Path = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"}
            };
            result.Images.Should().BeEquivalentTo(expected);
        }

        [TestClass]
        public class IfSourceTracksItemsIsEmpty
        {
            private static AlbumDetailsViewModel result;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                var source = CreateDomainAlbum();
                source.Tracks = new List<Track>();
                result = new AlbumDetailsViewModel(source);
            }

            [TestMethod]
            public void TracksAreEmpty() => result.Tracks.Should().BeEmpty();

            [TestMethod]
            public void NumberOfSongsIsZero() => result.NumberOfSongs.Should().Be(0);
        }

        [TestClass]
        public class IfSourceTracksExist
        {
            private static AlbumDetailsViewModel result;

            [ClassInitialize]
            public static void Initialize(TestContext _)
            {
                var source = CreateDomainAlbum();
                source.Tracks = new List<Track>
                {
                    new Track { Name = "Welcome Home", Number = 1},
                    new Track { Name = "Get Ready (Hot Machete)", Number = 2},
                    new Track { Name = "Communicate", Number = 3}
                };
                result = new AlbumDetailsViewModel(source);
            }

            [TestMethod]
            public void ConvertsTracks()
            {
                var expected = new List<TrackViewModel>
                {
                    new TrackViewModel {Name = "Welcome Home", Number = 1},
                    new TrackViewModel {Name = "Get Ready (Hot Machete)", Number = 2},
                    new TrackViewModel {Name = "Communicate", Number = 3}
                };
                result.Tracks.Should().BeEquivalentTo(expected);
            }

            [TestMethod]
            public void NumberOfSongsIsCorrect() => result.NumberOfSongs.Should().Be(3);
        }
    }
}
