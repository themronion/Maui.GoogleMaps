using Android.Gms.Maps;
using Maui.GoogleMaps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.GoogleMaps.Clustering
{   
    public partial class ClusterMapHandler : MapHandler
    {
        public static PropertyMapper<ClusteredMap, ClusterMapHandler> ClusterMapMapper = new(MapMapper)
        {

        };

        public ClusterMapHandler() : base(ClusterMapMapper)
        {
            
        }
        public override void InitLogics()
        {
            //   Map.ClusterOptions.MinimumClusterSize > 1
            ////    ? new ClusterLogic(this.Context, Config.GetBitmapdescriptionFactory(), OnClusteredMarkerCreating, OnClusteredMarkerCreated, OnClusteredMarkerDeleting, OnClusteredMarkerDeleted)
            base.InitLogics();
        }

        //public override void InitLogics() => Logics = new List<BaseLogic<GoogleMap>>
        //{
        //    new PolylineLogic(),
        //    new PolygonLogic(),
        //    new CircleLogic(),
        //    Map.ClusterOptions.MinimumClusterSize > 1
        //    ? new ClusterLogic(this.Context, Config.GetBitmapdescriptionFactory(), OnClusteredMarkerCreating, OnClusteredMarkerCreated, OnClusteredMarkerDeleting, OnClusteredMarkerDeleted)
        //    : new PinLogic(Config.GetBitmapdescriptionFactory(), OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
        //    new TileLayerLogic(),
        //    new GroundOverlayLogic(Config.GetBitmapdescriptionFactory())
        //};

    }
}
