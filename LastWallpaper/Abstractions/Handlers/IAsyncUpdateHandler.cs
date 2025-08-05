using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions.Handlers;

public interface IAsyncUpdateHandler
{
    Task HandleUpdateAsync( CancellationToken ct );
}