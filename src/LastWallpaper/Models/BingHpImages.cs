using System;

namespace LastWallpaper.Models
{
    public class BingHpImages
    {
        public ImageInfo[] Images { get; set; } = Array.Empty<ImageInfo>();

        public class ImageInfo
        {
            /// <summary>
            /// Publish date of the picture
            /// </summary>
            public string? StartDate { get; set; }
            /// <summary>
            /// Image without bing logo
            /// </summary>
            public string? UrlBase { get; set; }
            /// <summary>
            /// Copyrights of the image
            /// </summary>
            public string? CopyrightText { get; set; }
            /// <summary>
            /// Picture description. What is on the picture
            /// </summary>
            public string? Title { get; set; }
        }
    }
}
