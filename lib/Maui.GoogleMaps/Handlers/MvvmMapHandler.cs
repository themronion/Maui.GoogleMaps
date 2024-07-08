namespace Maui.GoogleMaps.Handlers;

public partial class MvvmMapHandler : MapHandler
{
    public static PropertyMapper<MvvmMap, MvvmMapHandler> MvvmMapMapper = new(MapMapper)
    {
#if ANDROID || IOS
        [nameof(MvvmMap.MaxZoomLevel)] = MapMinMaxZoomLevel,
        [nameof(MvvmMap.MinZoomLevel)] = MapMinMaxZoomLevel,
#endif
    };

    public MvvmMapHandler() : base(MvvmMapMapper)
    {

    }
}