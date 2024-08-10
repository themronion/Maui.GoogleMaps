
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Maui.GoogleMaps.Clustering.Platforms.Android;
using Maui.GoogleMaps.Handlers;
using Maui.GoogleMaps.Logics;
using Maui.GoogleMaps.Logics.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.GoogleMaps.Clustering
{
    public partial class ClusterMapHandler
    {

        /// <summary>
        /// Call when before marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the marker options.</param>
        protected virtual void OnClusteredMarkerCreating(Pin outerItem, MarkerOptions innerItem)
        {

        }

        /// <summary>
        /// Call when after marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the clustered marker.</param>
        protected virtual void OnClusteredMarkerCreated(Pin outerItem, ClusteredMarker innerItem)
        {
        }

        /// <summary>
        /// Call when before marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the clustered marker.</param>
        protected virtual void OnClusteredMarkerDeleting(Pin outerItem, ClusteredMarker innerItem)
        {
        }
        protected virtual void CreateNativeItem(Pin outerItem)
        {
        }
        /// <summary>
        /// Call when after marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the clustered marker.</param>
        /// 
        protected virtual void OnClusteredMarkerDeleted(Pin outerItem, ClusteredMarker innerItem)
        {
        }
        protected override void OnMapReady()
        {
            base.OnMapReady();

            var cluster =  VirtualView as ClusteredMap;

        }
   
    }
    public partial class ClusterMapHandler : MapHandler
    {
   

        public ClusterMapHandler()
        {
            
        }
        //protected override void OnMarkerCreated(Pin outerItem, Marker innerItem)
        //{
        //    base.OnMarkerCreated(outerItem, innerItem);
        //}
        //protected override void OnMarkerCreating(Pin outerItem, MarkerOptions innerItem)
        //{
        //    base.OnMarkerCreating(outerItem, innerItem);
        //}
        //public override void InitLogics() //=> Logics = new List<BaseLogic<GoogleMap>>
        //{
        //    Logics = = new List<BaseLogic<GoogleMap>> { 
        //    new PolylineLogic();
        //    new PolygonLogic();
        //    new CircleLogic();
        //    new ClusterLogic(this.Context, Config.GetBitmapdescriptionFactory(), OnClusteredMarkerCreating, OnClusteredMarkerCreated, OnClusteredMarkerDeleting, OnClusteredMarkerDeleted);
        //    new TileLayerLogic();
        //    new GroundOverlayLogic(Config.GetBitmapdescriptionFactory());
        //}
          public override void InitLogics() => Logics = new List<BaseLogic<GoogleMap>>
        {
            new PolylineLogic(),
            new PolygonLogic(),
            new CircleLogic(),
            new ClusterLogic(this.Context, Config.GetBitmapdescriptionFactory(), OnClusteredMarkerCreating, OnClusteredMarkerCreated, OnClusteredMarkerDeleting, OnClusteredMarkerDeleted),
            new TileLayerLogic(),
            new GroundOverlayLogic(Config.GetBitmapdescriptionFactory())
        };
        //        public override void InitLogics()
        //        {
        //            //Map.ClusterOptions.MinimumClusterSize > 1 
        //            new ClusterLogic(this.Context, Config.GetBitmapdescriptionFactory(), OnClusteredMarkerCreating, OnClusteredMarkerCreated, OnClusteredMarkerDeleting, OnClusteredMarkerDeleted);
        //#if ANDROID
        //            //new GoogleMaps.Logics.Android.PinLogic(Config.GetBitmapdescriptionFactory(), OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted);

        //#endif
        //            base.InitLogics();
        //        }

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
