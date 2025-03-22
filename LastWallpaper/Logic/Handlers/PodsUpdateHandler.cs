using LastWallpaper.Abstractions;
using LastWallpaper.Abstractions.Handlers;
using LastWallpaper.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Handlers;

public sealed class PodsUpdateHandler(
    IReadOnlyCollection<IPotdLoader> pods,
    IParameterizedUpdateHandler<FrontUpdateParameters> frontUpdateHandler,
    IResultsProcessor<IReadOnlyCollection<PodUpdateResult>, FrontUpdateParameters> resultsProcessor )
    : IAsyncUpdateHandler
{
    public async Task HandleUpdateAsync( CancellationToken ct )
    {
        var podUpdateTasks =
            _pods
            .Select( pod => pod.UpdateAsync( ct ) )
            .ToArray();

        Task.WaitAll( podUpdateTasks, ct );

        var updateResults =
            podUpdateTasks
            .Where( t => t.Result.IsSuccess )
            .Select( t => t.Result.Value )
            .ToArray();

        var frontParameters =
            await _resultsProcessor.ProcessResultsAsync( updateResults, ct );

        _frontUpdateHandler?.HandleUpdate( frontParameters, ct );
    }

    private readonly IReadOnlyCollection<IPotdLoader> _pods = pods;
    private readonly IParameterizedUpdateHandler<FrontUpdateParameters> _frontUpdateHandler = frontUpdateHandler;
    private readonly IResultsProcessor<IReadOnlyCollection<PodUpdateResult>, FrontUpdateParameters> _resultsProcessor = resultsProcessor;
}
