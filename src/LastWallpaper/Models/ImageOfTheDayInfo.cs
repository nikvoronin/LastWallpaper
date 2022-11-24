using static LastWallpaper.Models.BingHpImages;

namespace LastWallpaper.Models
{
    public class ImageOfTheDayInfo
    {
        public readonly string FileName;
        public readonly ImageInfo SourceInfo;

        public string? Description => SourceInfo.Title;
        public string? Copyright => SourceInfo.CopyrightText;

        public ImageOfTheDayInfo( string fileName, ImageInfo sourceInfo )
        {
            FileName = fileName;
            SourceInfo = sourceInfo;
        }
    }
}
