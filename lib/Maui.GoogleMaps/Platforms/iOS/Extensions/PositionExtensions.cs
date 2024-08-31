using CoreLocation;

namespace Maui.GoogleMaps.iOS.Extensions;

public static class PositionExtensions
{
    public static CLLocationCoordinate2D ToCoord(this Position self)
    {
        return new CLLocationCoordinate2D(self.Latitude, self.Longitude);
    }
}