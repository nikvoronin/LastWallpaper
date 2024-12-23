using System;
using System.Numerics;

namespace LastWallpaper.Logic.KMeans;

/// <summary>
/// Simple cluster initializer.
/// </summary>
public class KmInitializer : IKmInitializer
{
    /// <summary>
    /// Initialize first iteration by choosing random Centroid volume
    /// within the given array of volume.
    /// </summary>
    /// <param name="volume">
    /// Vector volume.s
    /// </param>
    /// <param name="random">
    /// Randomizer.
    /// </param>
    /// <returns>
    /// Initial cluster set.
    /// </returns>
    public KmCluster[] Initialize(
        Vector3[] volume, int numClusters,
        Random? random = null )
    {
        random ??= Random.Shared;

        KmCluster[] clusters = new KmCluster[numClusters];
        for (int i = 0; i < numClusters; i++)
            clusters[i] = new KmCluster( volume[random.Next( volume.Length )] );

        return clusters;
    }
}
