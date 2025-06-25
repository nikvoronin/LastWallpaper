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
    IParameterizedUpdateHandler<UiUpdateParameters> uiUpdateHandler,
    IResultsProcessor<IReadOnlyCollection<PodUpdateResult>, UiUpdateParameters> resultsProcessor )
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

        var uiParameters =
            await _resultsProcessor.ProcessResultsAsync( updateResults, ct );

        _uiUpdateHandler?.HandleUpdate( uiParameters, ct );
    }

    private readonly IReadOnlyCollection<IPotdLoader> _pods = pods;
    private readonly IParameterizedUpdateHandler<UiUpdateParameters> _uiUpdateHandler = uiUpdateHandler;
    private readonly IResultsProcessor<IReadOnlyCollection<PodUpdateResult>, UiUpdateParameters> _resultsProcessor = resultsProcessor;
}
