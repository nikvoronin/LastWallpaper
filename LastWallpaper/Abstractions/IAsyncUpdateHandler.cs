using System.Threading;
using System.Threading.Tasks;

namespace LastWallpaper.Abstractions;

public interface IAsyncUpdateHandler
{
    Task HandleUpdateAsync( CancellationToken ct );
}