using Albumify.Domain.Models;
using Albumify.Web.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Albumify.Web.UnitTests
{
    [TestClass]
    public class TheSearchArtistViewModel_WhenConvertingFromAnArtist
    {
        [TestMethod]
        public void ConvertsImages_IfAny()
        {
            var artist = new Artist
            {
                Name = "Norma Jean",
                ThirdPartyId = "55b0Gfm53udtGBs8mmNXrH",
                Images = new List<Image>
                {
                    new Image
                    {
                        Height = 640,
                        Url = "https://i.scdn.co/image/4f4fb36a3c564d0078be2e21962ee47724747b44",
                        Width = 640
                    },
                    new Image
                    {
                        Height = 320,
                        Url = "https://i.scdn.co/image/8717743daaf2e2b2f53008e5835232f9e90d5897",
                        Width = 320
                    },
                    new Image
                    {
                        Height = 160,
                        Url = "https://i.scdn.co/image/e99dfcc008ac74efced891cda956f6e4d0463260",
                        Width = 160
                    }
                }
            };
            var expected = new List<ImageViewModel>
            {
                new ImageViewModel
                {
                    Height = 640,
                    Path = "https://i.scdn.co/image/4f4fb36a3c564d0078be2e21962ee47724747b44",
                    Width = 640
                },
                new ImageViewModel
                {
                    Height = 320,
                    Path = "https://i.scdn.co/image/8717743daaf2e2b2f53008e5835232f9e90d5897",
                    Width = 320
                },
                new ImageViewModel
                {
                    Height = 160,
                    Path = "https://i.scdn.co/image/e99dfcc008ac74efced891cda956f6e4d0463260",
                    Width = 160
                }
            };
            var result = new SearchArtistViewModel(artist);
            result.Images.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void SetsImagesToEmpty_IfNoImages()
        {
            var artist = new Artist
            {
                Name = "Norma Jean",
                ThirdPartyId = "55b0Gfm53udtGBs8mmNXrH"
            };
            var result = new SearchArtistViewModel(artist);
            result.Images.Should().BeEmpty();
        }
    }
}
