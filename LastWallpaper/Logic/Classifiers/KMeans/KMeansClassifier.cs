using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace LastWallpaper.Logic.Classifiers.KMeans;

/// <summary>
/// Classifier of the K-Means algorithm.
/// </summary>
public class KMeansClassifier( IKmInitializer clusterInitializer )
{
    /// <summary>
    /// Classify array of volume into numCluster clusters.
    /// </summary>
    /// <param name="volume">Vector volume.</param>
    /// <param name="numClusters">Number of clusters to produce.</param>
    /// <returns>Array of clusters.</returns>
    public KmCluster[] Compute( Vector3[] volume, int numClusters )
    {
        var clusters =
            _clusterInitializer.Initialize( volume, numClusters );

        // TODO: add option for upper limit of iterations
        for (var iteration = 0; iteration < 1000; iteration++) {
            var newClusters =
                ArrangePointsParallel( volume, clusters );

            if (IsStable( clusters, newClusters ))
                return newClusters;

            clusters = newClusters;
        }

        return clusters;
    }

    /// <summary>
    /// Assign volume of the incoming array to the nodeCluster with the nearest centroid
    /// </summary>
    /// <param name="volume">
    /// Vector volume.
    /// </param>
    /// <param name="sourceClusters">
    /// Previous set of clusters.
    /// </param>
    /// <returns>
    /// Produced next set of clusters.
    /// </returns>
    private static KmCluster[] ArrangePointsParallel(
        Vector3[] volume,
        KmCluster[] sourceClusters )
    {
        var numClusters = sourceClusters.Length;

        var nextClusters = new KmCluster[numClusters];
        for (int i = 0; i < numClusters; i++)
            nextClusters[i] = new KmCluster();

        var numNodes = Environment.ProcessorCount; // TODO: limit threads number

        var nodes = new List<KmCluster[]>();
        for (var k = 0; k < numNodes; k++) {
            var newClusters = new KmCluster[nextClusters.Length];
            for (int i = 0; i < newClusters.Length; i++)
                newClusters[i] = new KmCluster();

            nodes.Add( newClusters );
        }

        Parallel.For( 0, numNodes, nodeIx => {
            var localCentroids =
                sourceClusters
                .Select( x => x.Centroid )
                .ToArray();

            var chunkLen = volume.Length / numNodes;
            var startIx = nodeIx * chunkLen;
            for (int px = startIx; px < startIx + chunkLen; px++) {
                var point = volume[px];

                // TODO? extract distance calculator into class
                var minDistance =
                    Vector3.Distance(
                        localCentroids[0],
                        point );

                var bestClusterIx = 0;
                for (int ix = 1; ix < sourceClusters.Length; ix++) {
                    var distance =
                        Vector3.Distance(
                            localCentroids[ix],
                            point );

                    if (distance < minDistance) {
                        minDistance = distance;
                        bestClusterIx = ix;
                    }
                }

                nodes[nodeIx][bestClusterIx]
                    .AddPoint( point );
            }
        } );

        for (var nodeIx = 0; nodeIx < nodes.Count; nodeIx++) {
            for (var clusterIx = 0; clusterIx < nextClusters.Length; clusterIx++) {
                var sourceCluster = nodes[nodeIx][clusterIx];
                for (var px = 0; px < sourceCluster.Points.Count; px++)
                    nextClusters[clusterIx].AddPoint( sourceCluster.Points[px] );
            }
        }

        return nextClusters;
    }

    /// <summary>
    /// Verify if the classification has converged by comparing the clusters.
    /// </summary>
    /// <param name="sourceClusters">
    /// Previous set of clusters.
    /// </param>
    /// <param name="nextClusters">
    /// Next produced set of clusters.
    /// </param>
    /// <returns>
    /// <see langword="true"> if converged,
    /// <see langword="false"> otherwise.
    /// </returns>
    private static bool IsStable(
        KmCluster[] sourceClusters,
        KmCluster[] nextClusters,
        float epsilon = .1f )
    {
        for (int i = 0; i < sourceClusters.Length; i++) {
            if (sourceClusters[i].Points.Count != nextClusters[i].Points.Count)
                return false;
        }

        for (int i = 0; i < sourceClusters.Length; i++) {
            var distance =
                Vector3.Distance(
                    sourceClusters[i].Centroid,
                    nextClusters[i].Centroid );

            if (distance > epsilon) return false;
        }

        return true;
    }

    private readonly IKmInitializer _clusterInitializer = clusterInitializer;
}
