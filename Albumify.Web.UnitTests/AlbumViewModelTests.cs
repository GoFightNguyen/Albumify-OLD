using Albumify.Domain.Models;
using Albumify.Spotify.Models;
using Albumify.Web.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albumify.Web.UnitTests
{
    [TestClass]
    public class TheAlbumViewModel_WhenConvertingFromASpotifySimplifiedAlbumObject
    {
        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreNull()
        {
            var source = new SimplifiedAlbumObject { Images = null };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreEmpty()
        {
            var source = new SimplifiedAlbumObject { Images = new List<ImageObject>() };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void ConvertsImages_IfSourceImagesExist()
        {
            var source = new SimplifiedAlbumObject
            {
                Images = new List<ImageObject>
                {
                    new ImageObject {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                    new ImageObject {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                    new ImageObject {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
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

    [TestClass]
    public class TheAlbumViewModel_WhenConvertingFromAnAlbum
    {
        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreNull()
        {
            var source = new Album { Images = null };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void SetsImagesToEmpty_IfSourceImagesAreEmpty()
        {
            var source = new Album { Images = new List<Image>() };
            var result = new AlbumViewModel(source);
            result.Images.Should().BeEmpty();
        }

        [TestMethod]
        public void ConvertsImages_IfSourceImagesExist()
        {
            var source = new Album
            {
                Images = new List<Image>
                {
                    new Image {Height = 640, Width = 640, Url = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                    new Image {Height = 300, Width = 300, Url = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"},
                    new Image {Height = 64, Width = 64, Url = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"}
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