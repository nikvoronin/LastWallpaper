using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic;

public class ResultsProcessor(
    IResourceManager resourceManager,
    AppSettings settings )
    : IResultsProcessor<IReadOnlyCollection<PodUpdateResult>, FrontUpdateParameters>
{
    public ValueTask<FrontUpdateParameters> ProcessResultsAsync(
        IReadOnlyCollection<PodUpdateResult> results,
        CancellationToken ct )
    {
        results = results.Select( MapCachedFile ).ToList();

        var hasNews = results.Count > 0;
        var uiTargets =
            hasNews ? UiUpdateTargets.All
            : UiUpdateTargets.NotifyIcon; // change icon from time to time

        PodUpdateResult result =
            results
            .FirstOrDefault()
            ?? _resourceManager
                .RestoreLastWallpaper()
                .ValueOrDefault;

        if (hasNews) {
            Task.Run( () => {
#if !DEBUG
                WindowsRegistry.SetWallpaper(
                    result.Filename,
                    _settings.WallpaperFit );
#endif
                _resourceManager.RememberLastWallpaper( result );
            }, ct );
        }
        else { // test if wallpaper was changed from external source
            var systemDesktopWallpaperLastWriteTime =
                new FileInfo( _resourceManager.SystemDesktopWallpaperFilename )
                .LastWriteTime;

            var useSystemDesktopWallpaper =
                result is null
                || (systemDesktopWallpaperLastWriteTime - result.Created)
                    .TotalSeconds > 1;

            if (useSystemDesktopWallpaper) {
                result =
                    new() {
                        PodType = PodType.Local,
                        Created = systemDesktopWallpaperLastWriteTime,
                        Filename = _resourceManager.SystemDesktopWallpaperFilename
                    };

                _resourceManager.RememberLastWallpaper( result );
            }
        }

        Debug.Assert( result is not null );

        return
            ValueTask.FromResult(
                new FrontUpdateParameters( uiTargets, result ) );
    }

    private PodUpdateResult MapCachedFile( PodUpdateResult podResult )
    {
        try {
            var albumImageFilename =
                _resourceManager.CreateAlbumFilename(
                    podResult.PodType, podResult.Created );

            var copyToAlbum =
                podResult.CopyToAlbum
                && podResult.Filename != albumImageFilename;

            if (copyToAlbum)
                File.Move( podResult.Filename, albumImageFilename );

            return
                podResult with {
                    Created = podResult.Created.Date + DateTime.Now.TimeOfDay,
                    Filename =
                        copyToAlbum ? albumImageFilename
                        : podResult.Filename
                };

        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }

        return podResult;
    }

    private readonly IResourceManager _resourceManager = resourceManager;
    private readonly AppSettings _settings = settings;
}
