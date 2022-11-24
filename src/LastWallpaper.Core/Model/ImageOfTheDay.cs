namespace LastWallpaper.Core.Model
{
    public class ImageOfTheDay
    {
        public readonly string FileName;
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? Copyright { get; init; }

        public ImageOfTheDay(string fileName)
        {
            FileName = fileName;
        }
    }
}
