using Maui.GoogleMaps.Clustering.Platforms.iOS.Clustering;
using Maui.GoogleMaps.Handlers;
using Maui.GoogleMaps.Logics.iOS;

namespace Maui.GoogleMaps.Clustering
{   
    public partial class ClusterMapHandler : MapHandler
    {

        public ClusterMapHandler()
        {

        }
        public override void InitLogics() => Logics =
        [
          new PolylineLogic(),
                    new PolygonLogic(),
                    new CircleLogic(),
                    new ClusterLogic(Config.ImageFactory, OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
                    new TileLayerLogic(),
                    new GroundOverlayLogic(Config.GetImageFactory())
        ];     
    }
}
