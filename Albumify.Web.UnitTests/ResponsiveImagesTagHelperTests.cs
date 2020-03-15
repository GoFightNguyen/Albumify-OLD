using Albumify.Web.Models;
using Albumify.Web.TagHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Web.UnitTests
{
    [TestClass]
    public class TheResponsiveImagesTagHelper_WhenProcessing
    {
        private TagHelperContext _context;
        private TagHelperOutput _output;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            _output = new TagHelperOutput(
                "responsive-images",
                new TagHelperAttributeList(),
                (_, __) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                }
            );
        }

        [TestMethod]
        public void CreatesPicture_ForMultipleImages()
        {
            // Arrange
            var images = new List<ImageViewModel>
            {
                new ImageViewModel {Height = 640, Width = 640, Path = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 64, Width = 64, Path = "https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656"},
                new ImageViewModel {Height = 300, Width = 300, Path = "https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656"}
            };

            // Act
            var sut = new ResponsiveImagesTagHelper
            {
                Alt = "Album Art",
                Images = images
            };
            sut.Process(_context, _output);

            // Assert
            var expected =
                "<source media='(max-width: 299px)' srcset='https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656'>" +
                "<source media='(max-width: 639px)' srcset='https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656'>" +
                "<source media='(min-width: 640px)' srcset='https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656'>" +
                "<img class='img-fluid' src='https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656' alt='Album Art'>";

            _output.TagName.Should().Be("picture");
            _output.Content.GetContent().Should().Be(expected);
            _output.TagMode.Should().Be(TagMode.StartTagAndEndTag);
        }

        [TestMethod]
        public void CreatesPicture_ForSingleImage()
        {
            // Arrange
            var images = new List<ImageViewModel>
            {
                new ImageViewModel {Height = 640, Width = 640, Path = "https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656"}
            };

            // Act
            var sut = new ResponsiveImagesTagHelper
            {
                Alt = "Album Art",
                Images = images
            };
            sut.Process(_context, _output);

            // Assert
            var expected =
                "<source media='(min-width: 640px)' srcset='https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656'>" +
                "<img class='img-fluid' src='https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656' alt='Album Art'>";

            _output.TagName.Should().Be("picture");
            _output.Content.GetContent().Should().Be(expected);
            _output.TagMode.Should().Be(TagMode.StartTagAndEndTag);
        }

        [TestMethod]
        public void DoesNotCreatePicture_ForZeroImages()
        {
            // Arrange
            // Act
            var sut = new ResponsiveImagesTagHelper
            {
                Alt = "Album Art",
                Images = new List<ImageViewModel>()
            };
            sut.Process(_context, _output);

            // Assert
            _output.TagName.Should().BeNullOrEmpty();
            _output.Content.GetContent().Should().BeEmpty();
        }
    }
}
