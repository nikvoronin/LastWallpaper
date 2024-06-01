using LastWallpaper.Models;

namespace LastWallpaper;

public interface IUpdateHandler
{
    void HandleUpdate( Imago imago );
}

public sealed class UpdateHandler : IUpdateHandler
{
    public void HandleUpdate( Imago imago )
    {
        ToastNotifications.ShowToast(
            imago.Filename,
            imago.Title,
            imago.Copyright );
    }
}
