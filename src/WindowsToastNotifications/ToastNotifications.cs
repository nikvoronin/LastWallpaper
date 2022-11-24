using LastWallpaper.Core.Model;
using Microsoft.Toolkit.Uwp.Notifications;

namespace LastWallpaper
{
    public class ToastNotifications
    {
        public const string ToastGroupName = "The Last Wallpaper";

        public static void Main() { }
        
        public static void OnImageUpdated( ImageOfTheDay info )
        {
            var toast = new ToastContentBuilder()
                .AddHeroImage( new Uri( info.FileName ) );

            if ( info.Title is not null )
                toast.AddText( info.Title );

            if ( info.Copyright is not null )
                toast.AddAttributionText( info.Copyright );
            
            toast.Show( toast => {
                toast.Group = ToastGroupName;
                toast.ExpirationTime = DateTime.Now.AddDays( 2 ); // TODO: add expiration as option
            } );
        }
    }
}