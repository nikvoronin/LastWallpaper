using LastWallpaper.Abstractions;
using LastWallpaper.Models;
using System;
using System.Collections.Generic;
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

        // TODO: share imagos with Selector-Processor to select the best one
        // TODO: use only images with resolution of display or appsettings defined only
        var updateResult =
            updateResults.FirstOrDefault()
            // then try to restore the last known wallpaper
            ?? _resourceManager
                .RestoreLastWallpaper()
                .ValueOrDefault
            // or use system desktop wallpaper
            ?? CreateDefaultLocalUpdateResult();

        if (hasNews) {
            Task.Run( () => {
#if !DEBUG
                WindowsRegistry.SetWallpaper(
                    updateResult.Filename,
                    _settings.WallpaperFit );
#endif
                _resourceManager.RememberLastWallpaper( updateResult );
            }, ct );
        }
        else { // test if wallpaper was changed from external source
            var deltaTime =
                (SystemDesktopWallpaperLastWriteTime - updateResult.Created)
                .TotalSeconds;

            if (deltaTime >= 0.0)
                updateResult = CreateDefaultLocalUpdateResult();
        }

        _frontUpdateHandler?.HandleUpdate(
            new FrontUpdateParameters(
                uiTargets,
                updateResult ),
            ct );

        return Task.CompletedTask;
    }

    private DateTime SystemDesktopWallpaperLastWriteTime =>
        new FileInfo( _resourceManager.SystemDesktopWallpaperFilename )
        .LastWriteTimeUtc;

    private PodUpdateResult CreateDefaultLocalUpdateResult() =>
        new() {
            PodName = "local", // TODO: add local pod
            Created = SystemDesktopWallpaperLastWriteTime,
            Filename = _resourceManager.SystemDesktopWallpaperFilename
        };

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
