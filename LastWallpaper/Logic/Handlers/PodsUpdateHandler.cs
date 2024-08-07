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
            .Select( t => RenameCachedFile( t.Result.Value, ct ) )
            // At the moment it is important to materialize enumerable
            // because we should rename all cached files.
            .ToList();

        // TODO: share news with Selector
        _frontUpdateHandler?.HandleUpdate(
            new(
                imagos.Count != 0,
                imagos.FirstOrDefault() ),
            ct );

        return Task.CompletedTask;
    }

    private Imago RenameCachedFile( Imago imago, CancellationToken ct )
    {
        ct.ThrowIfCancellationRequested();

        try {
            var albumImageFilename =
                _resourceManager.CreateAlbumFilename(
                    imago.PodName,
                    imago.Created );

            var cachedFile =
                imago.Filename != albumImageFilename;

            if (cachedFile)
                File.Move( imago.Filename, albumImageFilename );

            return
                cachedFile ? imago with { Filename = albumImageFilename }
                : imago;
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }

        return imago;
    }

    private readonly IReadOnlyCollection<IPotdLoader> _pods = pods;
    private readonly IParameterizedUpdateHandler<FrontUpdateParameters> _frontUpdateHandler = frontUpdateHandler;
    private readonly IResourceManager _resourceManager = resourceManager;
}
