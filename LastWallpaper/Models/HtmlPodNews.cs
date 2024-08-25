namespace LastWallpaper.Models;

public sealed class HtmlPodNews : PodNews
{
    public required string Url { get; init; }
    public required string Author { get; init; }
    public required string Title { get; init; }
}
