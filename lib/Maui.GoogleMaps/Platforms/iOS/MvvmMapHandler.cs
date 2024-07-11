using Google.Maps;

using Microsoft.Maui.Platform;

using UIKit;

namespace Maui.GoogleMaps.Handlers;

public partial class MvvmMapHandler : MapHandler
{
    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);

        platformView.MarkerInfoWindow = GetInfoContents;
    }

    protected virtual UIView GetInfoContents(MapView nativeMap, Marker marker)
    {
        var pin = Map.Pins.FirstOrDefault(p => (Marker)p.NativeObject == marker);

        var template = (Map as MvvmMap)?.InfoWindowTemplate;

        while (template is DataTemplateSelector selector)
        {
            template = selector.SelectTemplate(pin.BindingContext, null);
        }

        if (template?.CreateContent() is not View view) return null;

        view.BindingContext = pin.BindingContext;

        var platformView = view.ToPlatform(Map.Handler.MauiContext);

        var request = view.Measure(this.Map.Width - 20, double.PositiveInfinity, MeasureFlags.IncludeMargins);
        view.Layout(new Rect(0, 0, request.Request.Width, request.Request.Height));

        platformView.Bounds = view.Bounds;

        return platformView;
    }

    public static void MapMinMaxZoomLevel(MvvmMapHandler handler, MvvmMap map)
    {
        if (handler.NativeMap is null) return;

        handler.NativeMap.SetMinMaxZoom(map.MinZoomLevel, map.MaxZoomLevel);
    }
}