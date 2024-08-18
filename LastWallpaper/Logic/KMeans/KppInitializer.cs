using System;
using System.Numerics;

namespace LastWallpaper.Logic.KMeans;

/// <summary>
/// KMeans++ cluster initializer.
/// </summary>
public class KppInitializer : IKmInitializer
{
    /// <summary>
    /// Initialize first iteration by choosing first Centroid point at random
    /// and next ones with the K-Means++ algorithm.
    /// </summary>
    /// <param name="volume">
    /// Vector volume.
    /// </param>
    /// <param name="random">
    /// Randomizer.
    /// </param>
    /// <returns>
    /// Initial cluster set.
    /// </returns>
    public KmCluster[] Initialize(
        Vector3[] volume, int numClusters,
        Random? random = null)
    {
        random ??= Random.Shared;

        KmCluster[] clusters = new KmCluster[numClusters];

        clusters[0] = new KmCluster(volume[random.Next(volume.Length)]);

        for (int i = 1; i < numClusters; i++)
        {
            float accumulatedDistances = 0f;

            float[] accDistances = new float[volume.Length];

            for (int pointIdx = 0; pointIdx < volume.Length; pointIdx++)
            {
                float minDistance =
                    Vector3.Distance(
                        clusters[0].Centroid,
                        volume[pointIdx]);

                for (int clusterIdx = 1; clusterIdx < i; clusterIdx++)
                {
                    float currentDistance =
                        Vector3.Distance(
                            clusters[clusterIdx].Centroid,
                            volume[pointIdx]);

                    if (currentDistance < minDistance)
                        minDistance = currentDistance;
                }

                accumulatedDistances += minDistance * minDistance;
                accDistances[pointIdx] = accumulatedDistances;
            }

            float targetPoint = random.NextSingle() * accumulatedDistances;

            for (int pointIdx = 0; pointIdx < volume.Length; pointIdx++)
            {
                if (accDistances[pointIdx] >= targetPoint)
                {
                    clusters[i] = new KmCluster(volume[pointIdx]);
                    break;
                }
            }
        }

        return clusters;
    }
}
