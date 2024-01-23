using Android.Gms.Maps;
using NativeGroundOverlay = Android.Gms.Maps.Model.GroundOverlay;
using Android.Gms.Maps.Model;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Android.Factories;

namespace Maui.GoogleMaps.Logics.Android;

internal class GroundOverlayLogic : DefaultGroundOverlayLogic<NativeGroundOverlay, GoogleMap>
{
    protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

    private readonly IBitmapDescriptorFactory _bitmapDescriptorFactory;

    public GroundOverlayLogic(IBitmapDescriptorFactory bitmapDescriptorFactory)
    {
        _bitmapDescriptorFactory = bitmapDescriptorFactory;
    }

    internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap, oldMap, newNativeMap, newMap, handler);

        if (newNativeMap != null)
        {
            newNativeMap.GroundOverlayClick += OnGroundOverlayClick;
        }
    }

    internal override void Unregister(GoogleMap nativeMap, Map map)
    {
        if (nativeMap != null)
        {
            nativeMap.GroundOverlayClick -= OnGroundOverlayClick;
        }

        base.Unregister(nativeMap, map);
    }

    protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
    {
        var factory = _bitmapDescriptorFactory;
        var nativeDescriptor = factory.ToNative(outerItem.Icon, Handler.MauiContext);

        var opts = new GroundOverlayOptions()
            .PositionFromBounds(outerItem.Bounds.ToLatLngBounds())
            .Clickable(outerItem.IsClickable)
            .InvokeBearing(outerItem.Bearing)
            .InvokeImage(nativeDescriptor)
            .InvokeTransparency(outerItem.Transparency)
            .InvokeZIndex(outerItem.ZIndex);

        var overlay = NativeMap.AddGroundOverlay(opts);

        overlay.Visible = outerItem.IsVisible;

        // associate pin with marker for later lookup in event handlers
        outerItem.NativeObject = overlay;
        return overlay;
    }

    protected override NativeGroundOverlay DeleteNativeItem(GroundOverlay outerItem)
    {
        var nativeOverlay = outerItem.NativeObject as NativeGroundOverlay;
        if (nativeOverlay == null)
            return null;
        nativeOverlay.Remove();
        outerItem.NativeObject = null;

        return nativeOverlay;
    }

    void OnGroundOverlayClick(object sender, GoogleMap.GroundOverlayClickEventArgs e)
    {
        // clicked ground overlay
        var nativeItem = e.GroundOverlay;

        // lookup overlay
        var targetOuterItem = GetItems(Map).FirstOrDefault(
            outerItem => ((NativeGroundOverlay)outerItem.NativeObject).Id == nativeItem.Id);

        // only consider event handled if a handler is present.
        // Else allow default behavior of displaying an info window.
        targetOuterItem?.SendTap();
    }

    internal override void OnUpdateBearing(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Bearing = outerItem.Bearing;
    }

    internal override void OnUpdateBounds(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.SetPositionFromBounds(outerItem.Bounds.ToLatLngBounds()); 
    }

    internal override void OnUpdateIcon(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        var factory = _bitmapDescriptorFactory;
        var nativeDescriptor = factory.ToNative(outerItem.Icon, Handler.MauiContext);
        nativeItem.SetImage(nativeDescriptor);
    }

    internal override void OnUpdateIsClickable(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Clickable = outerItem.IsClickable;
    }

    internal override void OnUpdateTransparency(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Transparency = outerItem.Transparency;
    }

    internal override void OnUpdateZIndex(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.ZIndex = outerItem.ZIndex;
    }
}