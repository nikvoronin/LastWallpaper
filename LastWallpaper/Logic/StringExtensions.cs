using System;

namespace LastWallpaper.Logic;

public static class StringExtensions
{
    public static string? Truncate( this string text, int maxLength ) =>
        text?[..Math.Min( text.Length, maxLength )];
}
