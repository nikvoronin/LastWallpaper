using System.Threading;

namespace LastWallpaper.Abstractions;

public interface IParameterizedUpdateHandler<T>
{
    void HandleUpdate( T values, CancellationToken ct );
}