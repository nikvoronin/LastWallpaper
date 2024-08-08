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
    IResourceManager resourceManager )
    : IAsyncUpdateHandler
{
    public Task HandleUpdateAsync( CancellationToken ct )
    {
        var podUpdateTasks =
            _pods
            .Select( pod => pod.UpdateAsync( ct ) )
            .ToArray();

        Task.WaitAll( podUpdateTasks, ct );

        var imagos =
            podUpdateTasks
            .Where( t => t.Result.IsSuccess )
            .Select( t => MapCachedFile( t.Result.Value, ct ) )
            // Important to materialize enumerable
            // because we should map all cached files.
            .ToList();

        // TODO: share imagos with Selector to select the best one
        var imago = imagos.FirstOrDefault();
        _frontUpdateHandler?.HandleUpdate(
            new FrontUpdateParameters(
                updateWallpaper: imago is not null,
                imago ?? FileManager.LoadLastImago().ValueOrDefault ),
            ct );

        if (imago is not null)
            FileManager.SaveCurrentImago( imago );

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
}
