#nullable enable
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Utils.Clustering;
using Microsoft.Maui.Platform;

namespace Maui.GoogleMaps.Clustering;

/// <summary>Arguments passed to <see cref="GoogleMapsClusteringOptions.CreateAndroidClusterRenderer"/>.</summary>
public sealed class AndroidClusterRendererCreateContext
{
    public AndroidClusterRendererCreateContext(
        Context context,
        ClusteredMap clusteredMap,
        GoogleMap nativeMap,
        ClusterManager clusterManager,
        IMauiContext mauiContext)
    {
        Context = context;
        ClusteredMap = clusteredMap;
        NativeMap = nativeMap;
        ClusterManager = clusterManager;
        MauiContext = mauiContext;
    }

    public Context Context { get; }
    public ClusteredMap ClusteredMap { get; }
    public GoogleMap NativeMap { get; }
    public ClusterManager ClusterManager { get; }
    public IMauiContext MauiContext { get; }
}

public sealed partial class GoogleMapsClusteringOptions
{
    /// <summary>
    /// Optional factory for the Android cluster renderer. When null, the default <see cref="Platforms.Android.ClusterRenderer"/> is used.
    /// </summary>
    public Func<AndroidClusterRendererCreateContext, Platforms.Android.ClusterRenderer>? CreateAndroidClusterRenderer { get; set; }
}
