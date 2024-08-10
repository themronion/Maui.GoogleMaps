using Maui.GoogleMaps.Clustering.Platforms.iOS.Clustering;
using Maui.GoogleMaps.Handlers;
using Maui.GoogleMaps.Logics;
using Maui.GoogleMaps.Logics.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
