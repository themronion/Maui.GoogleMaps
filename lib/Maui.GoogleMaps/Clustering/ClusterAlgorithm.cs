// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/

namespace Maui.GoogleMaps.Clustering;

/// <summary>
/// Describe the available cluster algorithms.
/// </summary>
public enum ClusterAlgorithm
{
    NonHierarchicalDistanceBased,
    GridBased,
    /// <summary>
    /// Android only
    /// </summary>
    VisibleNonHierarchicalDistanceBased
}