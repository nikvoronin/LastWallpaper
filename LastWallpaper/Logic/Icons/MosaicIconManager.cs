using Colourful;
using LastWallpaper.Logic.Classifiers.KMeans;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace LastWallpaper.Logic.Icons;

public class MosaicIconManager : IconManager
{
    public override Icon CreateIcon( string sourceImagePath )
    {
        if (!File.Exists( sourceImagePath )) throw new FileNotFoundException();

        using var source = new Bitmap( sourceImagePath );
        var k = ResizedImageWidth / source.Width;
        using var resized =
            new Bitmap(
                (int)(source.Width * k),
                (int)(source.Height * k) );

        using var g = Graphics.FromImage( resized );
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        g.DrawImage( source, 0, 0, resized.Width, resized.Height );

        Vector3[] points = new Vector3[resized.Width * resized.Height];

        for (int y = 0; y < resized.Height; y++) {
            for (int x = 0; x < resized.Width; x++) {
                var pixel = resized.GetPixel( x, y );
                var lab = _rgb2lab.Convert( RGBColor.FromColor( pixel ) );
                points[y * resized.Width + x] =
                    new Vector3( (float)lab.L, (float)lab.a, (float)lab.b );
            }
        }

        var clusters =
            _classifier.Compute( points, SqrTilesNum * SqrTilesNum );

        using var result =
            new Bitmap(
                DefaultTrayIconSize.Width,
                DefaultTrayIconSize.Height );

        using var gr = Graphics.FromImage( result );

        int tileSize = result.Width / SqrTilesNum;

        for (int y = 0; y < SqrTilesNum; y++) {
            for (int x = 0; x < SqrTilesNum; x++) {
                var labMean = clusters[y * SqrTilesNum + x].Centroid;
                Brush brush =
                    new SolidBrush(
                        _lab2rgb.Convert(
                            new LabColor( labMean.X, labMean.Y, labMean.Z ) ) );

                gr.FillRectangle(
                    brush,
                    x * tileSize, y * tileSize,
                    tileSize, tileSize );
            }
        }

        return Icon.FromHandle( result.GetHicon() );
    }

    private readonly KMeansClassifier _classifier = new( new KppInitializer() );

    private readonly IColorConverter<RGBColor, LabColor> _rgb2lab =
        new ConverterBuilder()
        .FromRGB( RGBWorkingSpaces.sRGB )
        .ToLab( Illuminants.D65 )
        .Build();

    private readonly IColorConverter<LabColor, RGBColor> _lab2rgb =
        new ConverterBuilder()
        .FromLab( Illuminants.D65 )
        .ToRGB( RGBWorkingSpaces.sRGB )
        .Build();

    private const int SqrTilesNum = 3;
    private const float ResizedImageWidth = 400f;
}
