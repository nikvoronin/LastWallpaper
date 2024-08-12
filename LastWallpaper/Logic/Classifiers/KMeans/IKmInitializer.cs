using System;
using System.Numerics;

namespace LastWallpaper.Logic.Classifiers.KMeans;

public interface IKmInitializer
{
    public KmCluster[] Initialize(
        Vector3[] volume, int numClusters,
        Random? random = null);
}
