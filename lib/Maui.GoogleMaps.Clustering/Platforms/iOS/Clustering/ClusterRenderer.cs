// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah
using CoreAnimation;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Google.Maps.Utils;
using Google.Maps;
using UIKit;

namespace Maui.GoogleMaps.Clustering.Platforms.iOS.Clustering
{
    internal class ClusterRenderer : GMUDefaultClusterRenderer
    {
        private const double AnimationDuration = 0.5;

        private readonly MapView nativeMap;
        private readonly int minimumClusterSize;
        private float maxClusterZoom = 20;

        public ClusterRenderer(MapView mapView, IGMUClusterIconGenerator iconGenerator, int minimumClusterSize)
            : base(mapView, iconGenerator)
        {
            nativeMap = mapView;
            this.minimumClusterSize = minimumClusterSize;
        }

        public Marker GetMarker(ClusteredMarker clusteredMarker) =>
            Markers?.FirstOrDefault(m => ReferenceEquals(m.UserData as ClusteredMarker, clusteredMarker));

        public void SetUpdateMarker(ClusteredMarker clusteredMarker)
        {
            var marker = GetMarker(clusteredMarker);
            if (marker == null)
            {
                return;
            }

            marker.Position = new CLLocationCoordinate2D(clusteredMarker.Position.Latitude, clusteredMarker.Position.Longitude);
            marker.Title = clusteredMarker.Title;
            marker.Snippet = clusteredMarker.Snippet;
            marker.Draggable = clusteredMarker.Draggable;
            marker.Rotation = clusteredMarker.Rotation;
            marker.GroundAnchor = clusteredMarker.GroundAnchor;
            marker.InfoWindowAnchor = clusteredMarker.InfoWindowAnchor;
            marker.Flat = clusteredMarker.Flat;
            marker.Opacity = clusteredMarker.Opacity;
            marker.Icon = clusteredMarker.Icon;
        }

        public override bool ShouldRenderAsCluster(IGMUCluster cluster, float zoom)
        {
            return cluster.Count > (uint)minimumClusterSize && zoom <= maxClusterZoom;
        }

        [Export("markerWithPosition:from:userData:clusterIcon:animated:")]
        public Marker MarkerWithPosition(CLLocationCoordinate2D position, CLLocationCoordinate2D from,
            NSObject userData, UIImage clusterIcon,
            bool animated)
        {
            var initialPosition = animated ? from : position;
            var marker = userData is ClusteredMarker ?
                CreateSinglePin(userData, initialPosition)
                : CreateGroup(clusterIcon, initialPosition);

            marker.Position = initialPosition;
            marker.UserData = userData;
            marker.Map = nativeMap;

            if (animated)
            {
                AnimateMarker(position, marker);
            }

            return marker;
        }

        private static Marker CreateGroup(UIImage clusterIcon, CLLocationCoordinate2D initialPosition)
        {
            var marker = Marker.FromPosition(initialPosition);
            marker.Icon = clusterIcon;
            marker.GroundAnchor = new CGPoint(0.5, 0.5);
            marker.ZIndex = 1;
            return marker;
        }

        private static Marker CreateSinglePin(NSObject userData, CLLocationCoordinate2D initialPosition)
        {
            var clusteredMarker = userData as ClusteredMarker;
            var marker = Marker.FromPosition(initialPosition);
            marker.Icon = clusteredMarker.Icon;
            marker.Title = clusteredMarker.Title;
            marker.Snippet = clusteredMarker.Snippet;
            marker.Draggable = clusteredMarker.Draggable;
            marker.Rotation = clusteredMarker.Rotation;
            marker.GroundAnchor = clusteredMarker.GroundAnchor;
            marker.InfoWindowAnchor = clusteredMarker.InfoWindowAnchor;
            marker.Flat = clusteredMarker.Flat;
            marker.Opacity = clusteredMarker.Opacity;
            marker.ZIndex = clusteredMarker.ZIndex;
            return marker;
        }

        private static void AnimateMarker(CLLocationCoordinate2D position, Marker marker)
        {
            CATransaction.Begin();
            CATransaction.AnimationDuration = AnimationDuration;
            marker.Layer.Latitude = position.Latitude;
            marker.Layer.Longitude = position.Longitude;
            CATransaction.Commit();
        }
    }
}

