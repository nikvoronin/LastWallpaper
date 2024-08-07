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
    IReadOnlyCollection<IPotdLoader> _pods,
    IParameterizedUpdateHandler<FrontUpdateParameters> _frontUpdateHandler )
    : IAsyncUpdateHandler
{
    public async Task HandleUpdateAsync( CancellationToken ct )
    {
        var news = new Dictionary<string, Imago>();

        // TODO: try download async simultaneously
        foreach (var pod in _pods) {
            ct.ThrowIfCancellationRequested();

            var result = await pod.UpdateAsync( ct );
            if (result.IsFailed) continue;

            var imago = result.Value;

            // TODO: move transfer of temp files out of the download cycle
            try {
                var albumImageFilename =
                    Path.Combine(
                        FileManager.AlbumFolder,
                        $"{pod.Name}{imago.Created:yyyyMMdd}.jpeg" );

                var cachedFile =
                    imago.Filename != albumImageFilename;

                if (cachedFile)
                    File.Move( imago.Filename, albumImageFilename );

                news.TryAdd(
                    pod.Name,
                    cachedFile ? imago with { Filename = albumImageFilename }
                    : imago );
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        // TODO: share news with Selector
        _frontUpdateHandler?.HandleUpdate(
            new(
                news.Count > 0,
                news.Values.FirstOrDefault() ),
            ct );
    }
}
