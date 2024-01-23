﻿using Google.Maps;
using Maui.GoogleMaps.iOS.Extensions;
using NativeCircle = Google.Maps.Circle;
using Microsoft.Maui.Platform;

namespace Maui.GoogleMaps.Logics.iOS;

internal class CircleLogic : DefaultCircleLogic<NativeCircle, MapView>
{
    internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap,oldMap, newNativeMap, newMap, handler);

        if (newNativeMap != null)
        {
            newNativeMap.OverlayTapped += OnOverlayTapped;
        }
    }

    internal override void Unregister(MapView nativeMap, Map map)
    {
        if (nativeMap != null)
        {
            nativeMap.OverlayTapped -= OnOverlayTapped;
        }

        base.Unregister(nativeMap, map);
    }

    protected override IList<Circle> GetItems(Map map) => map.Circles;

    protected override NativeCircle CreateNativeItem(Circle outerItem)
    {
        var nativeCircle = NativeCircle.FromPosition(
            outerItem.Center.ToCoord(), outerItem.Radius.Meters);
        nativeCircle.StrokeWidth = outerItem.StrokeWidth;
        nativeCircle.StrokeColor = outerItem.StrokeColor.ToPlatform();
        nativeCircle.FillColor = outerItem.FillColor.ToPlatform();
        nativeCircle.Tappable = outerItem.IsClickable;
        nativeCircle.ZIndex = outerItem.ZIndex;

        outerItem.NativeObject = nativeCircle;
        nativeCircle.Map = NativeMap;
        return nativeCircle;
    }

    protected override NativeCircle DeleteNativeItem(Circle outerItem)
    {
        var nativeCircle = outerItem.NativeObject as NativeCircle;
        nativeCircle.Map = null;
        return nativeCircle;
    }

    void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
    {
        var targetOuterItem = GetItems(Map).FirstOrDefault(
            outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
        targetOuterItem?.SendTap();
    }

    protected override void OnUpdateStrokeWidth(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.StrokeWidth = outerItem.StrokeWidth;

    protected override void OnUpdateStrokeColor(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.StrokeColor = outerItem.StrokeColor.ToPlatform();

    protected override void OnUpdateFillColor(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.FillColor = outerItem.FillColor.ToPlatform();

    protected override void OnUpdateCenter(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.Position = outerItem.Center.ToCoord();

    protected override void OnUpdateRadius(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.Radius = outerItem.Radius.Meters;

    protected override void OnUpdateIsClickable(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.Tappable = outerItem.IsClickable;

    protected override void OnUpdateZIndex(Circle outerItem, NativeCircle nativeItem)
        => nativeItem.ZIndex = outerItem.ZIndex;
}

