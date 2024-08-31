// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah


using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Java.Lang;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
namespace Maui.GoogleMaps.Clustering.Platforms.Android
{
    public class ClusteredMarker(Pin outerItem) : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; } = new LatLng(outerItem.Position.Latitude, outerItem.Position.Longitude);
        public float Alpha { get; set; } = 1f - outerItem.Transparency;
        public bool Draggable { get; set; } = outerItem.IsDraggable;
        public bool Flat { get; set; } = outerItem.Flat;
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsInfoWindowShown { get; set; }
        public float Rotation { get; set; } = outerItem.Rotation;
        public string Snippet { get; set; } = outerItem.Address;
        public string Title { get; set; } = outerItem.Label;
        public bool Visible { get; set; } = outerItem.IsVisible;
        public float AnchorX { get; set; } = (float)outerItem.Anchor.X;
        public float AnchorY { get; set; } = (float)outerItem.Anchor.Y;

        //public float InfoWindowAnchorX { get; set; }

        //public float InfoWindowAnchorY { get; set; }

        public NativeBitmapDescriptor Icon { get; set; }
        public Float ZIndex { get; set; } = new Float(outerItem.ZIndex);
    }
}
