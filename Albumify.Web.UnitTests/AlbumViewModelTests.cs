using Albumify.Domain.Spotify;
using Albumify.Web.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albumify.Web.UnitTests
{
    [TestClass]
    public class TheAlbumViewModel_WhenConvertingsFromASpotifySimplifiedAlbumObject
    {
        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreNull()
        {
            var source = new SpotifySimplifiedAlbumObject { Images = null };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreEmpty()
        {
            var source = new SpotifySimplifiedAlbumObject { Images = new List<SpotifyImageObject>() };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void ConvertsImages_IfSourceImagesAreEmpty()
        {
            var source = new SpotifySimplifiedAlbumObject
            {
                Images = new List<SpotifyImageObject>
                {
                    new SpotifyImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                    new SpotifyImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                    new SpotifyImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
                }
            };
            var result = new AlbumViewModel(source);
            var expected = new List<ImageViewModel>
            {
                new ImageViewModel {Height = 640, Width = 640, Path = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 64, Width = 64, Path = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 300, Width = 300, Path = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"}
            };
            result.Images.Should().BeEquivalentTo(expected);
        }
    }
}
