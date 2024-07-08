namespace Maui.GoogleMaps.Handlers;

public partial class MvvmMapHandler : MapHandler
{
    public static PropertyMapper<MvvmMap, MvvmMapHandler> MvvmMapMapper = new(MapMapper)
    {
#if ANDROID || IOS
        [nameof(MvvmMap.MaxZoomLevel)] = MapZoomLevel,
        [nameof(MvvmMap.MinZoomLevel)] = MapZoomLevel,
#endif
    };

    public MvvmMapHandler() : base(MvvmMapMapper)
    {

    }
}