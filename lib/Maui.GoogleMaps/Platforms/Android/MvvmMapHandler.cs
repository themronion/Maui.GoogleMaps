using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Microsoft.Maui.Platform;

using static Android.Views.ViewGroup;

namespace Maui.GoogleMaps.Handlers;

public partial class MvvmMapHandler
{
    protected InfoWindowAdapter InfoWindowAdapter { get; set; }

    protected override void OnMapReady()
    {
        base.OnMapReady();

        MapMinMaxZoomLevel(this, VirtualView as MvvmMap);

        SetupInfoWindowAdapter();
    }

    protected virtual void SetupInfoWindowAdapter()
    {
        InfoWindowAdapter = new(VirtualView as MvvmMap);

        NativeMap.SetInfoWindowAdapter(InfoWindowAdapter);
    }

    public static void MapMinMaxZoomLevel(MvvmMapHandler handler, MvvmMap map)
    {
        if (handler.NativeMap is null) return;

        handler.NativeMap.SetMinZoomPreference(map.MinZoomLevel);
        handler.NativeMap.SetMaxZoomPreference(map.MaxZoomLevel);
    }
}

public class InfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
{
    protected MvvmMap Map { get; }

    public InfoWindowAdapter(MvvmMap map)
    {
        Map = map;
    }

    public virtual global::Android.Views.View GetInfoContents(Marker marker)
    {
        return null;
    }

    public virtual global::Android.Views.View GetInfoWindow(Marker marker)
    {
        var pin = Map.Pins.FirstOrDefault(p => ((Marker)p.NativeObject).Id == marker.Id);

        var template = Map.InfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(pin.BindingContext, null);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext;

        var platformView = view.ToPlatform(Map.Handler.MauiContext);

        var request = view.Measure(this.Map.Width - (int)platformView.Context.ToPixels(20), double.PositiveInfinity, MeasureFlags.IncludeMargins);

        platformView.LayoutParameters = new LayoutParams((int)platformView.Context.ToPixels(request.Request.Width), (int)platformView.Context.ToPixels(request.Request.Height));

        return platformView;
    }
}