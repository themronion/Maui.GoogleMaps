// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/

using System.Collections;
using System.ComponentModel;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Clustering.Algorithm;
using Android.Widget;

using Maui.GoogleMaps.Android;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Android.Factories;
using Maui.GoogleMaps.Logics;

namespace Maui.GoogleMaps.Clustering.Android;

internal class ClusterLogic : DefaultPinLogic<ClusteredMarker, GoogleMap>
{
    protected override IList<Pin> GetItems(Map map) => Map.Pins;

    private volatile bool onMarkerEvent = false;
    private Pin draggingPin;
    private volatile bool withoutUpdateNative;

    private ClusterManager clusterManager;
    private ClusterLogicListener clusterListener;

    private readonly Context context;
    private readonly IBitmapDescriptorFactory bitmapDescriptorFactory;
    private readonly Action<Pin, MarkerOptions> onMarkerCreating;
    private readonly Action<Pin, ClusteredMarker> onMarkerCreated;
    private readonly Action<Pin, ClusteredMarker> onMarkerDeleting;
    private readonly Action<Pin, ClusteredMarker> onMarkerDeleted;

    private global::Android.Gms.Maps.Model.CameraPosition previousCameraPostion;
    private ClusterRenderer clusterRenderer;

    private readonly Dictionary<string, Pin> itemsDictionary = new Dictionary<string, Pin>();

    public ClusterLogic(
        Context context,
        IBitmapDescriptorFactory bitmapDescriptorFactory,
        Action<Pin, MarkerOptions> onMarkerCreating,
        Action<Pin, ClusteredMarker> onMarkerCreated,
        Action<Pin, ClusteredMarker> onMarkerDeleting,
        Action<Pin, ClusteredMarker> onMarkerDeleted)
    {
        this.bitmapDescriptorFactory = bitmapDescriptorFactory;
        this.context = context;
        this.onMarkerCreating = onMarkerCreating;
        this.onMarkerCreated = onMarkerCreated;
        this.onMarkerDeleting = onMarkerDeleting;
        this.onMarkerDeleted = onMarkerDeleted;
    }

    internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap, oldMap, newNativeMap, newMap, handler);

        clusterManager = new ClusterManager(context, NativeMap)
        {
            Algorithm = Map.ClusterOptions.Algorithm switch
            {
                ClusterAlgorithm.GridBased => new GridBasedAlgorithm(),
                ClusterAlgorithm.VisibleNonHierarchicalDistanceBased => new NonHierarchicalViewBasedAlgorithm(
                                        context.Resources.DisplayMetrics.WidthPixels,
                                        context.Resources.DisplayMetrics.HeightPixels),
                _ => new NonHierarchicalDistanceBasedAlgorithm(),
            }
        };

        clusterListener = new ClusterLogicListener(Map, clusterManager, this);

        if (newNativeMap == null)
        {
            return;
        }

        Map.OnCluster = HandleClusterRequest;

        NativeMap.CameraIdle += NativeMapOnCameraIdle;
        NativeMap.SetOnMarkerClickListener(clusterManager);
        NativeMap.SetOnInfoWindowClickListener(clusterManager);
        newNativeMap.MarkerDragStart += OnMarkerDragStart;
        newNativeMap.MarkerDragEnd += OnMarkerDragEnd;
        newNativeMap.MarkerDrag += OnMarkerDrag;

        clusterRenderer = new ClusterRenderer(context,
            Map,
            NativeMap,
            clusterManager,
            handler.MauiContext);
        clusterManager.Renderer = clusterRenderer;

        clusterManager.SetOnClusterClickListener(clusterListener);
        clusterManager.SetOnClusterInfoWindowClickListener(clusterListener);
        clusterManager.SetOnClusterItemClickListener(clusterListener);
        clusterManager.SetOnClusterItemInfoWindowClickListener(clusterListener);
    }

    private void NativeMapOnCameraIdle(object sender, EventArgs e)
    {
        var cameraPosition = NativeMap.CameraPosition;
        if (previousCameraPostion == null || Math.Abs(previousCameraPostion.Zoom - cameraPosition.Zoom) > 0.001)
        {
            previousCameraPostion = NativeMap.CameraPosition;
            clusterManager.Cluster();
        }
    }

    internal override void Unregister(GoogleMap nativeMap, Map map)
    {
        if (nativeMap != null)
        {
            NativeMap.CameraIdle -= NativeMapOnCameraIdle;
            NativeMap.SetOnMarkerClickListener(null);
            NativeMap.SetOnInfoWindowClickListener(null);
            nativeMap.MarkerDrag -= OnMarkerDrag;
            nativeMap.MarkerDragEnd -= OnMarkerDragEnd;
            nativeMap.MarkerDragStart -= OnMarkerDragStart;

            clusterListener.Dispose();
            clusterManager.Dispose();
        }

        base.Unregister(nativeMap, map);
    }

    protected override ClusteredMarker CreateNativeItem(Pin outerItem)
    {
        var opts = new MarkerOptions()
            .SetPosition(new LatLng(outerItem.Position.Latitude, outerItem.Position.Longitude))
            .SetTitle(outerItem.Label)
            .SetSnippet(outerItem.Address)
            .SetSnippet(outerItem.Address)
            .Draggable(outerItem.IsDraggable)
            .SetRotation(outerItem.Rotation)
            .Anchor((float)outerItem.Anchor.X, (float)outerItem.Anchor.Y)
            .InvokeZIndex(outerItem.ZIndex)
            .Flat(outerItem.Flat)
            .SetAlpha(1f - outerItem.Transparency);

        if (outerItem.Icon != null)
        {
            var factory = bitmapDescriptorFactory ?? DefaultBitmapDescriptorFactory.Instance;
            var nativeDescriptor = factory.ToNative(outerItem.Icon, Handler.MauiContext);
            opts.SetIcon(nativeDescriptor);
        }

        onMarkerCreating(outerItem, opts);

        var marker = new ClusteredMarker(outerItem);
        if (outerItem.Icon != null)
        {
            var factory = bitmapDescriptorFactory ?? DefaultBitmapDescriptorFactory.Instance;
            var nativeDescriptor = factory.ToNative(outerItem.Icon, Handler.MauiContext);
            marker.Icon = nativeDescriptor;
        }
        if (outerItem?.Icon?.Type == BitmapDescriptorType.View)
        {
            marker.Visible = false;
            TransformMauiViewToAndroidBitmap(outerItem, marker);
            return marker;
        }
        else
        {
            marker.Visible = outerItem.IsVisible;
        }

        AddMarker(outerItem, marker);
        return marker;
    }

    private void AddMarker(Pin outerItem, ClusteredMarker marker)
    {
        outerItem.NativeObject = marker;
        onMarkerCreated(outerItem, marker);
        clusterManager.AddItem(marker);
        itemsDictionary.Add(marker.Id, outerItem);
    }

    protected override ClusteredMarker DeleteNativeItem(Pin outerItem)
    {
        if (outerItem.NativeObject is not ClusteredMarker marker)
        {
            return null;
        }

        onMarkerDeleting(outerItem, marker);
        clusterManager.RemoveItem(marker);
        outerItem.NativeObject = null;

        if (ReferenceEquals(Map.SelectedPin, outerItem))
        {
            Map.SelectedPin = null;
        }

        itemsDictionary.Remove(marker.Id);
        onMarkerDeleted(outerItem, marker);
        return marker;
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
            clusterRenderer.SetUpdateMarker((sender as Pin).NativeObject as ClusteredMarker);
        }
    }

    public Pin LookupPin(ClusteredMarker marker)
        => marker.Id != null ? itemsDictionary[marker.Id] : null;

    public void HandleClusterRequest()
        => clusterManager.Cluster();

    internal override void OnMapPropertyChanged(string propertyName)
    {
        if (propertyName == Map.SelectedPinProperty.PropertyName)
        {
            if (!onMarkerEvent)
            {
                if (Map.SelectedPin == null)
                {
                    foreach (var outerItem in GetItems(Map))
                    {
                        (outerItem.NativeObject as Marker)?.HideInfoWindow();
                    }
                }
                else
                {
                    var targetPin = LookupPin(Map.SelectedPin.NativeObject as ClusteredMarker);
                    (targetPin?.NativeObject as Marker)?.ShowInfoWindow();
                }
            }

            Map.SendSelectedPinChanged(Map.SelectedPin);
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
            nativeItem.Position = outerItem.Position.ToLatLng();
        }
    }

    protected override void OnUpdateType(Pin outerItem, ClusteredMarker nativeItem)
    {
    }

    protected override void OnUpdateIcon(Pin outerItem, ClusteredMarker nativeItem)
    {
        if (outerItem.Icon != null && outerItem.Icon.Type == BitmapDescriptorType.View)
        {
            TransformMauiViewToAndroidBitmap(outerItem, nativeItem);
        }
        else
        {
            var factory = bitmapDescriptorFactory ?? DefaultBitmapDescriptorFactory.Instance;
            var nativeDescriptor = factory.ToNative(outerItem.Icon, Handler.MauiContext);
            nativeItem.Icon = nativeDescriptor;
            nativeItem.AnchorX = 0.5f;
            nativeItem.AnchorY = 0.5f;
            //nativeItem.InfoWindowAnchorX = 0.5f;
            //nativeItem.InfoWindowAnchorY = 0.5f;
        }
    }

    private void TransformMauiViewToAndroidBitmap(Pin outerItem, ClusteredMarker nativeItem)
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

        var nativeView = Utils.ConvertMauiToNative(iconView, Handler.MauiContext);
        var otherView = new FrameLayout(nativeView.Context);
        nativeView.LayoutParameters = new FrameLayout.LayoutParams(Utils.DpToPx((float)iconView.WidthRequest), Utils.DpToPx((float)iconView.HeightRequest));
        otherView.AddView(nativeView);
        nativeItem.Icon = Utils.ConvertViewToBitmapDescriptor(otherView);
        nativeItem.AnchorX = (float)iconView.AnchorX;
        nativeItem.AnchorY = (float)iconView.AnchorY;
        nativeItem.Visible = true;

        if (outerItem.NativeObject == null)
        {
            AddMarker(outerItem, nativeItem);
        }
    }

    protected override void OnUpdateIsDraggable(Pin outerItem, ClusteredMarker nativeItem)
        => nativeItem.Draggable = outerItem?.IsDraggable ?? false;

    protected override void OnUpdateRotation(Pin outerItem, ClusteredMarker nativeItem)
        => nativeItem.Rotation = outerItem?.Rotation ?? 0f;

    protected override void OnUpdateIsVisible(Pin outerItem, ClusteredMarker nativeItem)
    {
        var isVisible = outerItem?.IsVisible ?? false;
        nativeItem.Visible = isVisible;

        if (!isVisible && ReferenceEquals(Map.SelectedPin, outerItem))
        {
            Map.SelectedPin = null;
        }
    }

    protected override void OnUpdateAnchor(Pin outerItem, ClusteredMarker nativeItem)
    {
        nativeItem.AnchorX = (float)outerItem.Anchor.X;
        nativeItem.AnchorY = (float)outerItem.Anchor.Y;
    }

    protected override void OnUpdateFlat(Pin outerItem, ClusteredMarker nativeItem)
        => nativeItem.Flat = outerItem.Flat;

    protected override void OnUpdateInfoWindowAnchor(Pin outerItem, ClusteredMarker nativeItem)
    {
        //nativeItem.InfoWindowAnchorX = (float)outerItem.InfoWindowAnchor.X;
        //nativeItem.InfoWindowAnchorY = (float)outerItem.InfoWindowAnchor.Y;
    }

    protected override void OnUpdateZIndex(Pin outerItem, ClusteredMarker nativeItem)
        => nativeItem.ZIndex = new Java.Lang.Float(outerItem.ZIndex);

    protected override void OnUpdateTransparency(Pin outerItem, ClusteredMarker nativeItem)
        => nativeItem.Alpha = 1f - outerItem.Transparency;

    private void OnMarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
    {
        var clusterItem = clusterRenderer.GetClusterItem(e.Marker);
        draggingPin = LookupPin((ClusteredMarker)clusterItem);

        if (draggingPin == null)
        {
            return;
        }

        UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
        Map.SendPinDragStart(draggingPin);
    }

    private void OnMarkerDrag(object sender, GoogleMap.MarkerDragEventArgs e)
    {
        if (draggingPin == null)
        {
            return;
        }

        UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
        Map.SendPinDragging(draggingPin);
    }

    private void OnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
    {
        if (draggingPin != null)
        {
            UpdatePositionWithoutMove(draggingPin, e.Marker.Position.ToPosition());
            RefreshClusterItem();
            Map.SendPinDragEnd(draggingPin);
            draggingPin = null;
        }
        withoutUpdateNative = false;
    }

    private void RefreshClusterItem()
    {
        clusterManager.RemoveItem((Java.Lang.Object)draggingPin.NativeObject);
        clusterManager.AddItem(CreateNativeItem(draggingPin));
        clusterManager.Cluster();
    }
}