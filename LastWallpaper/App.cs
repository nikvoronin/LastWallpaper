using System;
using System.Threading;

namespace LastWallpaper;

public static class App
{
    public static SynchronizationContext? UiContext;

    public static void TryInvokeOnUiContext( Action action )
        => UiContext?.Post( _ => action(), null );
}
