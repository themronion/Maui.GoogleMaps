using CoreLocation;

namespace Maui.GoogleMaps.iOS.Extensions;

public static class CLLocationCoordinate2DExtensions
{
    public static Position ToPosition(this CLLocationCoordinate2D self)
    {
        return new Position(self.Latitude, self.Longitude);
    }
}