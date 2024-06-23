using System.Threading;

namespace LastWallpaper.Abstractions;

public interface IUpdateHandler
{
    void HandleUpdate( CancellationToken ct );
}