using Avalonia;
using LastWallpaper.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using File = System.IO.File;
using Path = System.IO.Path;

namespace LastWallpaper
{
    public sealed class BingLoader
    {
        private const string BingImageListUrlTemplate = 
            "https://bingwallpaper.microsoft.com/api/BWC/getHPImages?screenWidth={0}&screenHeight={1}";
        private const int StartImmediately = 0;

        private static readonly TimeSpan CheckNewImagePeriod = TimeSpan.FromMinutes( 57 );
        private static readonly int CheckNewImagePeriodMs = (int)CheckNewImagePeriod.TotalMilliseconds;
        private static readonly HttpClient _httpClient = new();

        private Timer? _timer;
        private readonly string _cacheFolder;
        private readonly string _bingImageListUrl;

        public event EventHandler<ImageOfTheDayInfo>? ImageUpdated;

        public BingLoader( PixelSize screenSize, string cacheFolder )
        {
            _bingImageListUrl = string.Format( 
                BingImageListUrlTemplate, 
                screenSize.Width, screenSize.Height );
            _cacheFolder = cacheFolder;
        }

        public void StartPoll()
        {
            var cacheFolderExists = Directory.Exists( _cacheFolder );
            if ( !cacheFolderExists )
                Directory.CreateDirectory( _cacheFolder );

            _timer = new( OnTimerTick, null, StartImmediately, CheckNewImagePeriodMs );
        }

        private void OnTimerTick( object? state ) => Update();

        public void Update()
            => Task.Run( async () => {
                try {
                    await UpdateInternal();
                }
                catch (Exception e ) { 
                    // TODO: log exception
                }
            } );

        private async Task UpdateInternal()
        {
            // get actual list of the fresh images ------------------------
            var images = await _httpClient.GetFromJsonAsync<BingHpImages>( _bingImageListUrl );

            var emptyImageList =
                images is null
                || images.Images.Length < 1;
            if ( emptyImageList ) return;

            var imageOfTheDay = images!.Images[0];

            // download image localy ------------------------

            var imageOfTheDayFileName =
                Path.Combine(
                    _cacheFolder,
                    $"{imageOfTheDay.StartDate}.jpg" );

            var imageAlreadyExists = File.Exists( imageOfTheDayFileName );
            if ( imageAlreadyExists ) return;

            using ( var s = await _httpClient.GetStreamAsync( imageOfTheDay.UrlBase ) ) {
                using var fs = new FileStream( imageOfTheDayFileName, FileMode.Create );
                await s.CopyToAsync( fs );
            }

            // notice everyone ------------------------

            OnImageUpdated( 
                new ImageOfTheDayInfo(
                    Path.GetFullPath( imageOfTheDayFileName ),
                    imageOfTheDay )
                );
        }

        private void OnImageUpdated( ImageOfTheDayInfo info )
        {
            var eh = ImageUpdated;
            eh?.Invoke( this, info );
        }

        internal void StopPoll()
        {
            try {
                // disable timer
                _timer?.Change( Timeout.Infinite, Timeout.Infinite );
            }
            catch { }
        }
    }
}
