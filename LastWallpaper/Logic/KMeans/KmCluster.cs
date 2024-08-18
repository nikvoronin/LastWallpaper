using System.Collections.Generic;
using System.Numerics;

namespace LastWallpaper.Logic.KMeans;

/// <summary>
/// Vector cluster for the K-Means algorithm.
/// </summary>
public class KmCluster
{
    public Vector3 Centroid
    {
        get
        {
            if (_numPoints != _points.Count)
            {
                _numPoints = _points.Count;
                _centroid = _accumulator / _numPoints;
            }

            return _centroid;
        }
    }

    public IReadOnlyList<Vector3> Points => _points;

    /// <summary>
    /// Create cluster with empty volume.
    /// </summary>
    public KmCluster()
    {
        _points = [];
    }

    /// <summary>
    /// Create cluster with a given centroid vector.
    /// </summary>
    /// <param name="centroid">
    /// Centroid vector of the cluster.
    /// </param>
    public KmCluster(Vector3 centroid)
    {
        _points = [centroid];
        _accumulator = centroid;
    }

    /// <summary>
    /// Add vector to a cluster.
    /// </summary>
    /// <param name="point">
    /// New vector to append to the cluster.
    /// </param>
    public void AddPoint(Vector3 point)
    {
        _accumulator += point;
        _points.Add(point);
    }

    private Vector3 _accumulator = Vector3.Zero;
    private Vector3 _centroid = Vector3.Zero;
    private int _numPoints;

    private readonly List<Vector3> _points;
}
