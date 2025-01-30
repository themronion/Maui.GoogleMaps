// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah
using System.Collections;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Google.Maps;
using Google.Maps.Utils;
using UIKit;
using Maui.GoogleMaps.iOS.Extensions;
using Maui.GoogleMaps.iOS.Factories;
using Maui.GoogleMaps.Logics;
using Maui.GoogleMaps.Clustering.Logics;

namespace Maui.GoogleMaps.Clustering.Platforms.iOS.Clustering
{
    internal class ClusterLogic : DefaultPinLogic<ClusteredMarker, MapView>
    {
        protected override IList<Pin> GetItems(Map map) => Map.Pins;
        public ClusteredMap ClusteredMapp => (ClusteredMap)Map;
        private GMUClusterManager clusterManager;

        private bool onMarkerEvent;
        private Pin draggingPin;
        private volatile bool withoutUpdateNative;

        private readonly Action<Pin, Marker> onMarkerCreating;
        private readonly Action<Pin, Marker> onMarkerCreated;
        private readonly Action<Pin, Marker> onMarkerDeleting;
        private readonly Action<Pin, Marker> onMarkerDeleted;
        private readonly IImageFactory imageFactory;
        private ClusterRenderer clusterRenderer;

        private readonly Dictionary<NSObject, Pin> itemsDictionary = new Dictionary<NSObject, Pin>();

        public ClusterLogic(
            IImageFactory imageFactory,
            Action<Pin, Marker> onMarkerCreating,
            Action<Pin, Marker> onMarkerCreated,
            Action<Pin, Marker> onMarkerDeleting,
            Action<Pin, Marker> onMarkerDeleted)
        {
            this.imageFactory = imageFactory;
            this.onMarkerCreating = onMarkerCreating;
            this.onMarkerCreated = onMarkerCreated;
            this.onMarkerDeleting = onMarkerDeleting;
            this.onMarkerDeleted = onMarkerDeleted;
        }

        public override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap, IElementHandler handler)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap, handler);

            clusterRenderer = new ClusterRenderer(newNativeMap,
                new ClusterIconGenerator(ClusteredMapp.ClusterOptions, handler.MauiContext),
                ClusteredMapp.ClusterOptions.MinimumClusterSize);

            clusterManager = new GMUClusterManager(newNativeMap,
                GetClusterAlgorithm(ClusteredMapp),
                clusterRenderer);

            ClusteredMapp.OnCluster = HandleClusterRequest;

            if (newNativeMap == null)
            {
                return;
            }

            newNativeMap.InfoTapped += OnInfoTapped;
            newNativeMap.InfoLongPressed += OnInfoLongPressed;
            newNativeMap.TappedMarker = HandleGmsTappedMarker;
            newNativeMap.InfoClosed += InfoWindowClosed;
            newNativeMap.DraggingMarkerStarted += DraggingMarkerStarted;
            newNativeMap.DraggingMarkerEnded += DraggingMarkerEnded;
            newNativeMap.DraggingMarker += DraggingMarker;
        }

        private static IGMUClusterAlgorithm GetClusterAlgorithm(ClusteredMap clusteredNewMap) => clusteredNewMap.ClusterAlgorithmType switch
        {
            ClusterAlgorithm.GridBased => new GMUGridBasedClusterAlgorithm(),
            ClusterAlgorithm.VisibleNonHierarchicalDistanceBased => throw new NotSupportedException("VisibleNonHierarchicalDistanceBased is only supported on Android"),
            _ => new GMUNonHierarchicalDistanceBasedAlgorithm(),
        };

        public override void Unregister(MapView nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.DraggingMarker -= DraggingMarker;
                nativeMap.DraggingMarkerEnded -= DraggingMarkerEnded;
                nativeMap.DraggingMarkerStarted -= DraggingMarkerStarted;
                nativeMap.InfoClosed -= InfoWindowClosed;
                nativeMap.TappedMarker = null;
                nativeMap.InfoTapped -= OnInfoTapped;
            }

            ClusteredMapp.OnCluster = null;

            base.Unregister(nativeMap, map);
        }

        protected override ClusteredMarker CreateNativeItem(Pin outerItem)
        {
            var nativeMarker = new ClusteredMarker
            {
                Position = outerItem.Position.ToCoord(),
                Title = outerItem.Label,
                Snippet = outerItem.Address ?? string.Empty,
                Draggable = outerItem.IsDraggable,
                Rotation = outerItem.Rotation,
                GroundAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y),
                //  InfoWindowAnchor = new CGPoint(outerItem.InfoWindowAnchor.X, outerItem.InfoWindowAnchor.Y),
                Flat = outerItem.Flat,
                ZIndex = outerItem.ZIndex,
                Opacity = 1f - outerItem.Transparency
            };

            if (outerItem.Icon != null)
            {
                var factory = imageFactory ?? DefaultImageFactory.Instance;
                nativeMarker.Icon = factory.ToUIImage(outerItem.Icon, Handler.MauiContext);
            }

            onMarkerCreating(outerItem, nativeMarker);

            outerItem.NativeObject = nativeMarker;

            clusterManager.AddItem(nativeMarker);
            itemsDictionary.Add(nativeMarker, outerItem);
            OnUpdateIconView(outerItem, nativeMarker);
            onMarkerCreated(outerItem, nativeMarker);

            return nativeMarker;
        }

        protected override ClusteredMarker DeleteNativeItem(Pin outerItem)
        {
            if (outerItem?.NativeObject == null)
            {
                return null;
            }

            var nativeMarker = outerItem.NativeObject as ClusteredMarker;

            onMarkerDeleting(outerItem, nativeMarker);

            nativeMarker.Map = null;

            clusterManager.RemoveItem(nativeMarker);

            if (ReferenceEquals(Map.SelectedPin, outerItem))
            {
                Map.SelectedPin = null;
            }

            itemsDictionary.Remove(nativeMarker);
            onMarkerDeleted(outerItem, nativeMarker);

            return nativeMarker;
        }

        protected override void AddItems(IList newItems)
        {
            base.AddItems(newItems);
            clusterManager.Cluster();
        }

        protected override void RemoveItems(IList oldItems)
        {
            base.RemoveItems(oldItems);
            clusterManager.Cluster();
        }

        protected override void ResetItems()
        {
            base.ResetItems();
            clusterManager.Cluster();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            if (e.PropertyName != Pin.PositionProperty.PropertyName)
            {
                clusterRenderer.SetUpdateMarker((ClusteredMarker)(sender as Pin)?.NativeObject);
            }
        }

        public override void OnMapPropertyChanged(string propertyName)
        {
            if (propertyName == Map.SelectedPinProperty.PropertyName)
            {
                if (!onMarkerEvent)
                {
                    NativeMap.SelectedMarker = Map.SelectedPin?.NativeObject as ClusteredMarker;
                }

                Map.SendSelectedPinChanged(Map.SelectedPin);
            }
        }

        private Pin LookupPin(Marker marker)
            => marker.UserData != null ? itemsDictionary[marker.UserData] : null;

        private void HandleClusterRequest()
            => clusterManager.Cluster();

        private void OnInfoTapped(object sender, GMSMarkerEventEventArgs e)
        {
            var targetPin = LookupPin(e.Marker);

            targetPin?.SendTap();

            if (targetPin != null)
            {
                Map.SendInfoWindowClicked(targetPin);
            }
        }

        private void OnInfoLongPressed(object sender, GMSMarkerEventEventArgs e)
        {
            var targetPin = LookupPin(e.Marker);

            if (targetPin != null)
            {
                Map.SendInfoWindowLongClicked(targetPin);
            }
        }

        private bool HandleGmsTappedMarker(MapView mapView, Marker marker)
        {
            if (marker?.UserData is IGMUCluster cluster)
            {
                var count = (int)cluster.Count;
                var items = cluster.Items.Select(x => itemsDictionary[(ClusteredMarker)x]);
                var position = new Position(cluster.Position.Latitude, cluster.Position.Longitude);
                ClusteredMapp.SendClusterClicked(count, items, position);
            
                return true;
            }

            var targetPin = LookupPin(marker);

            if (Map.SendPinClicked(targetPin))
            {
                return true;
            }

            try
            {
                onMarkerEvent = true;
                if (targetPin != null && !ReferenceEquals(targetPin, Map.SelectedPin))
                {
                    Map.SelectedPin = targetPin;
                }
            }
            finally
            {
                onMarkerEvent = false;
            }

            return false;
        }


        private void InfoWindowClosed(object sender, GMSMarkerEventEventArgs e)
        {
            var targetPin = LookupPin(e.Marker);

            try
            {
                onMarkerEvent = true;
                if (targetPin != null && ReferenceEquals(targetPin, Map.SelectedPin))
                {
                    Map.SelectedPin = null;
                }
            }
            finally
            {
                onMarkerEvent = false;
            }
        }

        private void DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
        {
            draggingPin = LookupPin(e.Marker);

            if (draggingPin != null)
            {
                UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
                Map.SendPinDragStart(draggingPin);
            }
        }

        private void DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
        {
            if (draggingPin != null)
            {
                UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
                RefreshClusterItem();
                Map.SendPinDragEnd(draggingPin);
                draggingPin = null;
            }
        }

        private void RefreshClusterItem()
        {
            Map.Pins.Remove(draggingPin);
            Map.Pins.Add(draggingPin);
            clusterManager.Cluster();
        }

        private void DraggingMarker(object sender, GMSMarkerEventEventArgs e)
        {
            if (draggingPin != null)
            {
                UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
                Map.SendPinDragging(draggingPin);
            }
        }

        private void UpdatePositionWithoutMove(Pin pin, Position position)
        {
            try
            {
                withoutUpdateNative = true;
                pin.Position = position;
            }
            finally
            {
                withoutUpdateNative = false;
            }
        }

        protected override void OnUpdateAddress(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Snippet = outerItem.Address;

        protected override void OnUpdateLabel(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Title = outerItem.Label;

        protected override void OnUpdatePosition(Pin outerItem, ClusteredMarker nativeItem)
        {
            if (!withoutUpdateNative)
            {
                nativeItem.Position = outerItem.Position.ToCoord();
            }
        }

        protected override void OnUpdateType(Pin outerItem, ClusteredMarker nativeItem)
        {
        }

        protected override void OnUpdateIcon(Pin outerItem, ClusteredMarker nativeItem)
        {
            if (outerItem.Icon.Type == BitmapDescriptorType.View)
            {
                OnUpdateIconView(outerItem, nativeItem);
            }
            else if (nativeItem?.Icon != null)
            {
                nativeItem.Icon = DefaultImageFactory.Instance.ToUIImage(outerItem.Icon, Handler.MauiContext);
            }
        }

        protected override void OnUpdateIsDraggable(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Draggable = outerItem?.IsDraggable ?? false;

        private void OnUpdateIconView(Pin outerItem, ClusteredMarker nativeItem)
        {
            if ((outerItem?.Icon?.Type) != BitmapDescriptorType.View)
            {
                return;
            }

            var iconView = outerItem.Icon.View();

            if (iconView == null)
            {
                return;
            }

            NativeMap.InvokeOnMainThread(() =>
            {
                var nativeView = Utils.ConvertMauiToNative(iconView, Handler.MauiContext);
                nativeView.BackgroundColor = UIColor.Clear;
                nativeItem.GroundAnchor = new CGPoint(iconView.AnchorX, iconView.AnchorY);
                nativeItem.Icon = Utils.ConvertViewToImage(nativeView);
            });
        }

        protected override void OnUpdateRotation(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Rotation = outerItem?.Rotation ?? 0f;

        protected override void OnUpdateIsVisible(Pin outerItem, ClusteredMarker nativeItem)
        {
            if (outerItem?.IsVisible ?? false)
            {
                nativeItem.Map = NativeMap;
            }
            else
            {
                nativeItem.Map = null;
                if (ReferenceEquals(Map.SelectedPin, outerItem))
                {
                    Map.SelectedPin = null;
                }
            }
        }

        protected override void OnUpdateAnchor(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.GroundAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);

        protected override void OnUpdateFlat(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Flat = outerItem.Flat;

        protected override void OnUpdateInfoWindowAnchor(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.InfoWindowAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);

        protected override void OnUpdateZIndex(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.ZIndex = outerItem.ZIndex;

        protected override void OnUpdateTransparency(Pin outerItem, ClusteredMarker nativeItem)
            => nativeItem.Opacity = 1f - outerItem.Transparency;
    }

}
