using Android.App;

namespace Maui.GoogleMaps.Android.Extensions;

internal static class ActivityExtensions
{
    public static float GetScaledDensity(this Activity self)
    {
        return self.Resources.DisplayMetrics.Density;
    }
}