using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Maui.GoogleMaps.Android;
using Microsoft.Maui.Platform;
using NativePolyline = Android.Gms.Maps.Model.Polyline;
using NativePatternItem = Android.Gms.Maps.Model.PatternItem;

namespace Maui.GoogleMaps.Logics.Android;

public class PolylineLogic : DefaultPolylineLogic<NativePolyline, GoogleMap>
{
    protected override IList<Polyline> GetItems(Map map) => map.Polylines;

    public override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap, oldMap, newNativeMap, newMap, handler);

        if (newNativeMap != null)
        {
            newNativeMap.PolylineClick += OnPolylineClick;
        }
    }

    public override void Unregister(GoogleMap nativeMap, Map map)
    {
        if (nativeMap != null)
        {
            nativeMap.PolylineClick -= OnPolylineClick;
        }

        base.Unregister(nativeMap, map);
    }

    protected override NativePolyline CreateNativeItem(Polyline outerItem)
    {
        var opts = new PolylineOptions();

        foreach (var p in outerItem.Positions)
            opts.Add(new LatLng(p.Latitude, p.Longitude));

        opts.InvokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
        opts.InvokeColor(outerItem.StrokeColor.ToPlatform());
        opts.Clickable(outerItem.IsClickable);
        opts.InvokeZIndex(outerItem.ZIndex);
        opts.InvokePattern(GenerateLinePattern(outerItem));

        var nativePolyline = NativeMap.AddPolyline(opts);

        // associate pin with marker for later lookup in event handlers
        outerItem.NativeObject = nativePolyline;
        outerItem.SetOnPositionsChanged((polyline, e) =>
        {
            var native = polyline.NativeObject as NativePolyline;
            native.Points = polyline.Positions.ToLatLngs();
        });

        return nativePolyline;
    }

    protected override NativePolyline DeleteNativeItem(Polyline outerItem)
    {
        outerItem.SetOnPositionsChanged(null);

        var nativeShape = outerItem.NativeObject as NativePolyline;
        if (nativeShape == null)
            return null;

        nativeShape.Remove();
        outerItem.NativeObject = null;
        return nativeShape;
    }

    void OnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs e)
    {
        // clicked polyline
        var nativeItem = e.Polyline;

        // lookup pin
        var targetOuterItem = GetItems(Map).FirstOrDefault(
            outerItem => ((NativePolyline)outerItem.NativeObject).Id == nativeItem.Id);

        // only consider event handled if a handler is present.
        // Else allow default behavior of displaying an info window.
        targetOuterItem?.SendTap();
    }

    internal override void OnUpdateIsClickable(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.Clickable = outerItem.IsClickable;
    }

    internal override void OnUpdateStrokeColor(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.Color = outerItem.StrokeColor.ToPlatform();
    }

    internal override void OnUpdateStrokeWidth(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.Width = outerItem.StrokeWidth;
    }

    internal override void OnUpdateZIndex(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.ZIndex = outerItem.ZIndex;
    }

    internal override void OnUpdateLinePattern(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.Pattern = GenerateLinePattern(outerItem);
    }

    internal List<NativePatternItem> GenerateLinePattern(Polyline line)
    {
        if (line.StrokePattern == null || line.StrokePattern.Type == LineTypes.Straight)
            return null;
        else
        {
            var pattern = new List<NativePatternItem>();
            for (int i = 0; i < line.StrokePattern.Pattern.Count; i++)
            {
                var itemLength = line.StrokePattern.Pattern[i];
                if (i % 2 == 0)
                    pattern.Add(line.StrokePattern.Type == LineTypes.Dashed ? (NativePatternItem)new Dash(itemLength) : new Dot());
                else
                    pattern.Add(new Gap(itemLength));
            }

            return pattern;
        }
    }
}