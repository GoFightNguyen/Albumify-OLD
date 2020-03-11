using Albumify.Web.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Albumify.Web.TagHelpers
{
    /// <summary>
    /// This approach is using the art direction approach to responsive images.
    /// https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Responsive_images
    /// </summary>
    public class ResponsiveImagesTagHelper : TagHelper
    {
        public List<ImageViewModel> Images { get; set; }
        public string Alt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            /* Example of what the desired HTML is:
             
               <picture>
                    <source media="(max-width: 299px)" srcset="https://i.scdn.co/image/ab67616d00004851d50eac8c4023cf2b40413656">
                    <source media="(max-width: 639px)" srcset="https://i.scdn.co/image/ab67616d00001e02d50eac8c4023cf2b40413656">
                    <source media="(min-width: 640px)" srcset="https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656">
                    <img src="https://i.scdn.co/image/ab67616d0000b273d50eac8c4023cf2b40413656" alt="album art">
                </picture>
            */

            if (!Images.Any())
            {
                output.SuppressOutput();
                return;
            }

            var content = GenerateContent();
            output.TagName = "picture";
            output.Content.SetHtmlContent(content);
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        private string GenerateContent()
        {
            var sb = new StringBuilder();
            var imagesWithMediaConditions = CalculateMediaConditions(Images);
            
            foreach (var (mediaCondition, image) in imagesWithMediaConditions)
                sb.Append($"<source media='{mediaCondition}' srcset='{image.Path}'>");
            
            sb.Append($"<img src='{imagesWithMediaConditions.Last().image.Path}' alt='{Alt}'>");
            
            return sb.ToString();
        }

        private static List<(string mediaCondition, ImageViewModel image)> CalculateMediaConditions(List<ImageViewModel> images)
        {
            var orderImages = images.OrderBy(i => i.Width).ToList();
            var imagesWithMediaConditions = new List<(string, ImageViewModel)>();

            for (var i = 0; i < orderImages.Count; i++)
            {
                var current = orderImages[i];
                string mediaCondition;
                if (i != orderImages.Count - 1)
                {
                    var nextImage = orderImages[i + 1];
                    mediaCondition = $"(max-width: {nextImage.Width - 1}px)";
                }
                else
                {
                    mediaCondition = $"(min-width: {current.Width}px)";
                }

                imagesWithMediaConditions.Add((mediaCondition, current));
            }

            return imagesWithMediaConditions;
        }
    }
}
