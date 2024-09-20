using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Handlers;

public sealed class PodsUpdateHandler(
    IReadOnlyCollection<IPotdLoader> pods,
    IParameterizedUpdateHandler<FrontUpdateParameters> frontUpdateHandler,
    IResourceManager resourceManager,
    AppSettings settings )
    : IAsyncUpdateHandler
{
    public Task HandleUpdateAsync( CancellationToken ct )
    {
        var podUpdateTasks =
            _pods
            .Select( pod => pod.UpdateAsync( ct ) )
            .ToArray();

        Task.WaitAll( podUpdateTasks, ct );

        var updateResults =
            podUpdateTasks
            .Where( t => t.Result.IsSuccess )
            .Select( t => MapCachedFile( t.Result.Value, ct ) )
            // Important to materialize enumerable
            // because we should map all cached files.
            .ToList();

        var hasNews = updateResults.Count > 0;
        var uiTargets =
            hasNews ? UiUpdateTargets.All
            : UiUpdateTargets.NotifyIcon; // change icon from time to time

        PodUpdateResult result =
            updateResults
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
                .LastWriteTimeUtc;

            var useSystemDesktopWallpaper =
                result is null
                || (systemDesktopWallpaperLastWriteTime - result.Created)
                    .TotalSeconds > 1;

            if (useSystemDesktopWallpaper) {
                result =
                    new() {
                        PodName = "local", // TODO? add local pod
                        Created = systemDesktopWallpaperLastWriteTime,
                        Filename = _resourceManager.SystemDesktopWallpaperFilename
                    };

                _resourceManager.RememberLastWallpaper( result );
            }
        }

        Debug.Assert( result is not null );

        _frontUpdateHandler?.HandleUpdate(
            new FrontUpdateParameters(
                uiTargets,
                result ),
            ct );

        return Task.CompletedTask;
    }

    private PodUpdateResult MapCachedFile(
        PodUpdateResult imago,
        CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();

        try {
            var albumImageFilename =
                _resourceManager.CreateAlbumFilename(
                    imago.PodName, imago.Created );

            var copyToAlbum =
                imago.CopyToAlbum
                && imago.Filename != albumImageFilename;

            if (copyToAlbum)
                File.Move( imago.Filename, albumImageFilename );

            return
                imago with {
                    Created = imago.Created.Date + DateTime.Now.TimeOfDay,
                    Filename =
                        copyToAlbum ? albumImageFilename
                        : imago.Filename
                };

        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }

        return imago;
    }

    private readonly IReadOnlyCollection<IPotdLoader> _pods = pods;
    private readonly IParameterizedUpdateHandler<FrontUpdateParameters> _frontUpdateHandler = frontUpdateHandler;
    private readonly IResourceManager _resourceManager = resourceManager;
    private readonly AppSettings _settings = settings;
}
