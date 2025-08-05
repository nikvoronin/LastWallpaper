using System.Threading;

namespace LastWallpaper.Abstractions.Handlers;

public interface IParameterizedUpdateHandler<T>
{
    void HandleUpdate( T values, CancellationToken ct );
}