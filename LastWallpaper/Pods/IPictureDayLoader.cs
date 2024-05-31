﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Pods;

public interface IPictureDayLoader
{
    /// <summary>
    /// Name of the POD loader.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Updates and downloads pictures of the day.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Collection of pathes to downloaded images or empty collection otherwise.
    /// </returns>
    Task<IReadOnlyCollection<string>> UpdateAsync(
        CancellationToken ct );
}