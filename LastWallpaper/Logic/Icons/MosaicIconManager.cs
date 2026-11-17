using Colourful;
using KMeans;
using KMeans.DistanceEstimators;
using KMeans.Initializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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

        using (var g = Graphics.FromImage( resized )) {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage( source, 0, 0, resized.Width, resized.Height );
        }

        var (points, weights) = ExtractWeightedLabPoints( resized );

        var clusterCount =
            Math.Min( SqrtNumTiles * SqrtNumTiles, weights.Length );

        var clusters =
            _clusterizer.Compute( points, dimension: 3, clusterCount, weights );

        using var result =
            new Bitmap(
                DefaultTrayIconSize.Width,
                DefaultTrayIconSize.Height );

        using var gr = Graphics.FromImage( result );

        var w = result.Width;
        var h = result.Height;

        for (var clusterIx = 0; clusterIx < clusters.ClusterCount; clusterIx++) {
            var labMean = clusters.Centroid( clusterIx );
            var color =
                _lab2rgb.Convert(
                    new LabColor( labMean[0], labMean[1], labMean[2] ) );

            // Edges from the full size: the remainder of w / SqrtNumTiles
            // goes to the tiles instead of leaving a transparent strip.
            var col = clusterIx % SqrtNumTiles;
            var row = clusterIx / SqrtNumTiles;
            var x0 = col * w / SqrtNumTiles;
            var x1 = (col + 1) * w / SqrtNumTiles;
            var y0 = row * h / SqrtNumTiles;
            var y1 = (row + 1) * h / SqrtNumTiles;

            using var brush = new SolidBrush( color );
            gr.FillRectangle( brush, x0, y0, x1 - x0, y1 - y0 );
        }

        return Icon.FromHandle( result.GetHicon() );
    }

    /// <summary>
    /// Read pixels of the bitmap and merge identical colors into weighted points.
    /// </summary>
    /// <returns>
    /// Lab colors, row-major: unique colors × 3,
    /// and the number of pixels of every color.
    /// </returns>
    private (float[] Points, int[] Weights) ExtractWeightedLabPoints( Bitmap bitmap )
    {
        var data =
            bitmap.LockBits(
                new Rectangle( 0, 0, bitmap.Width, bitmap.Height ),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb );

        byte[] pixels;
        int stride;
        try {
            stride = data.Stride;
            pixels = new byte[stride * data.Height];
            Marshal.Copy( data.Scan0, pixels, 0, pixels.Length );
        }
        finally {
            bitmap.UnlockBits( data );
        }

        var colorCounts = new Dictionary<int, int>();
        for (var y = 0; y < bitmap.Height; y++) {
            var rowIx = y * stride;
            for (var x = 0; x < bitmap.Width; x++) {
                var px = rowIx + x * 4; // B, G, R, A
                var rgb = pixels[px + 2] << 16 | pixels[px + 1] << 8 | pixels[px];

                ref var count =
                    ref CollectionsMarshal.GetValueRefOrAddDefault( colorCounts, rgb, out _ );
                count++;
            }
        }

        var points = new float[colorCounts.Count * 3];
        var weights = new int[colorCounts.Count];
        var ix = 0;
        foreach (var (rgb, count) in colorCounts) {
            var lab =
                _rgb2lab.Convert(
                    RGBColor.FromRGB8Bit(
                        (byte)(rgb >> 16),
                        (byte)(rgb >> 8),
                        (byte)rgb ) );

            points[ix * 3] = (float)lab.L;
            points[ix * 3 + 1] = (float)lab.a;
            points[ix * 3 + 2] = (float)lab.b;
            weights[ix] = count;
            ix++;
        }

        return (points, weights);
    }

    private readonly KMeansProcessor<float, EuclideanDistance<float>> _clusterizer =
        new( new KppInitializer<float>() );

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
