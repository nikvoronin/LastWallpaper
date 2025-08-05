using System.Text;

namespace LastWallpaper.Pods.Wikimedia.Models;

public static class WmQueryTemplates
{
    public static readonly CompositeFormat PotdUrlFormat =
        CompositeFormat.Parse(
            QueryBase + "&prop=imageinfo&iiprop=url&titles={0}" );

    public static readonly CompositeFormat PotdCreditsFormat =
        CompositeFormat.Parse(
            QueryBase + "&prop=imageinfo&iiprop=extmetadata&titles={0}" );

    public const string QueryBase =
        "https://en.wikipedia.org/w/api.php?action=query&format=json&formatversion=2";

    public static readonly CompositeFormat PotdFilenameFormat =
        CompositeFormat.Parse(
            QueryBase + "&prop=images&titles=Template:POTD/{0}" );
}
