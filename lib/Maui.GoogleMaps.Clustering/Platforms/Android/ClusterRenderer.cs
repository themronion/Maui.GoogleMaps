// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Clustering.View;
using Maui.GoogleMaps.Android.Factories;
using Microsoft.Maui.Platform;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace Maui.GoogleMaps.Clustering.Platforms.Android
{
    public class ClusterRenderer : DefaultClusterRenderer
    {
        public ClusteredMap map;
        protected IMauiContext mauiContext;
        private readonly Dictionary<string, NativeBitmapDescriptor> disabledBucketsCache = [];
        private readonly Dictionary<string, NativeBitmapDescriptor> enabledBucketsCache = [];

        public ClusterRenderer(Context context,
            ClusteredMap map,
            GoogleMap nativeMap,
            ClusterManager manager,
            IMauiContext mauiContext)
            : base(context, nativeMap, manager)
        {
            this.map = map;
            MinClusterSize = map.ClusterOptions.MinimumClusterSize;

            this.mauiContext = mauiContext;
        }

        public void SetUpdateMarker(ClusteredMarker clusteredMarker)
        {
            var marker = GetMarker(clusteredMarker);
            if (marker == null)
            {
                return;
            }

            marker.Position = new LatLng(clusteredMarker.Position.Latitude, clusteredMarker.Position.Longitude);
            marker.Title = clusteredMarker.Title;
            marker.Snippet = clusteredMarker.Snippet;
            marker.Draggable = clusteredMarker.Draggable;
            marker.Rotation = clusteredMarker.Rotation;
            marker.SetAnchor(clusteredMarker.AnchorX, clusteredMarker.AnchorY);
            //marker.SetInfoWindowAnchor(clusteredMarker.InfoWindowAnchorX, clusteredMarker.InfoWindowAnchorY);
            marker.Flat = clusteredMarker.Flat;
            marker.Alpha = clusteredMarker.Alpha;
            marker.SetIcon(clusteredMarker.Icon);
        }

        protected override void OnBeforeClusterRendered(ICluster cluster, MarkerOptions options)
        {
            if (map.ClusterOptions.RendererCallback != null)
            {
                var descriptorFromCallback = map.ClusterOptions.RendererCallback(map.ClusterOptions.EnableBuckets ? GetClusterText(cluster) : cluster.Size.ToString());

                options.SetIcon(GetIcon(cluster, descriptorFromCallback));
            }
            else if (map.ClusterOptions.RendererImage != null)
            {
                options.SetIcon(GetIcon(cluster, map.ClusterOptions.RendererImage));
            }
            else
            {
                base.OnBeforeClusterRendered(cluster, options);
            }
        }

        private NativeBitmapDescriptor GetIcon(ICluster cluster, BitmapDescriptor descriptor)
        {
            var clusterText = GetClusterText(cluster);

            NativeBitmapDescriptor icon;

            if (map.ClusterOptions.EnableBuckets)
            {
                enabledBucketsCache.TryGetValue(clusterText, out icon);
            }
            else
            {
                disabledBucketsCache.TryGetValue(clusterText, out icon);
            }

            if (icon == null)
            {
                icon = DefaultBitmapDescriptorFactory.Instance.ToNative(descriptor, mauiContext);

                if (map.ClusterOptions.EnableBuckets)
                {
                    enabledBucketsCache.Add(clusterText, icon);
                }
                else
                {
                    disabledBucketsCache.Add(clusterText, icon);
                }
            }

            return icon;
        }

        protected override void OnBeforeClusterItemRendered(Java.Lang.Object marker, MarkerOptions options)
        {
            var clusteredMarker = marker as ClusteredMarker;

            options.SetTitle(clusteredMarker.Title)
                .SetSnippet(clusteredMarker.Snippet)
                .SetSnippet(clusteredMarker.Snippet)
                .Draggable(clusteredMarker.Draggable)
                .SetRotation(clusteredMarker.Rotation)
                .Anchor(clusteredMarker.AnchorX, clusteredMarker.AnchorY)
                //  .InfoWindowAnchor(clusteredMarker.InfoWindowAnchorX, clusteredMarker.InfoWindowAnchorY)
                .SetAlpha(clusteredMarker.Alpha)
                .Flat(clusteredMarker.Flat);

            if (clusteredMarker.Icon != null)
            {
                options.SetIcon(clusteredMarker.Icon);
            }

            base.OnBeforeClusterItemRendered(marker, options);
        }

        protected override int GetBucket(ICluster cluster)
            => cluster.Size <= map.ClusterOptions.Buckets[0] ? cluster.Size : map.ClusterOptions.Buckets[BucketIndexForSize(cluster.Size)];

        public override int GetColor(int size)
            => map.ClusterOptions.BucketColors[BucketIndexForSize(size)].ToPlatform();

        private string GetClusterText(ICluster cluster)
            => GetClusterText(cluster.Size);

        protected override string GetClusterText(int size)
        {
            if (map.ClusterOptions.EnableBuckets)
            {
                var buckets = map.ClusterOptions.Buckets;
                var bucketIndex = BucketIndexForSize(size);

                return size < buckets[0] ? size.ToString() : $"{buckets[bucketIndex]}+";
            }
            else
            {
                return size.ToString();
            }
        }

        private int BucketIndexForSize(int size)
        {
            uint index = 0;
            var buckets = map.ClusterOptions.Buckets;

            while (index + 1 < buckets.Length && buckets[index + 1] <= size)
            {
                ++index;
            }

            return (int)index;
        }
    }
}
