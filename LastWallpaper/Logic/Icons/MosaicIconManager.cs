﻿using Colourful;
using LastWallpaper.Logic.KMeans;
using System.Drawing;
using System.IO;
using System.Linq;
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

        var points = new Vector3[resized.Width * resized.Height];

        for (var y = 0; y < resized.Height; y++) {
            for (var x = 0; x < resized.Width; x++) {
                var pixel = resized.GetPixel( x, y );
                var lab = _rgb2lab.Convert( RGBColor.FromColor( pixel ) );
                points[y * resized.Width + x] =
                    new Vector3( (float)lab.L, (float)lab.a, (float)lab.b );
            }
        }

        var clusters =
            _clusterizer.Compute( points, SqrtNumTiles * SqrtNumTiles );

        using var result =
            new Bitmap(
                DefaultTrayIconSize.Width,
                DefaultTrayIconSize.Height );

        using var gr = Graphics.FromImage( result );

        var tileSize = result.Width / SqrtNumTiles;

        foreach (var clusterIx in Enumerable.Range( 0, clusters.Length )) {
            var labMean = clusters[clusterIx].Centroid;
            var color =
                _lab2rgb.Convert(
                    new LabColor( labMean.X, labMean.Y, labMean.Z ) );

            var x = clusterIx % SqrtNumTiles;
            var y = clusterIx / SqrtNumTiles;
            gr.FillRectangle(
                new SolidBrush( color ),
                x * tileSize, y * tileSize,
                tileSize, tileSize );
        }

        return Icon.FromHandle( result.GetHicon() );
    }

    private readonly KMeansProcessor _clusterizer = new( new KppInitializer() );

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

    private const int SqrtNumTiles = 3;
    private const float ResizedImageWidth = 400f;
}
