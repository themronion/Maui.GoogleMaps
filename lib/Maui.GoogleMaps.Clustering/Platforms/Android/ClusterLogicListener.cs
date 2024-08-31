// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/
// Original author code from https://github.com/sferhah
//
using Android.Gms.Maps.Utils.Clustering;

namespace Maui.GoogleMaps.Clustering.Platforms.Android
{
    internal class ClusterLogicListener : Java.Lang.Object,
     ClusterManager.IOnClusterClickListener,
     ClusterManager.IOnClusterItemClickListener,
     ClusterManager.IOnClusterInfoWindowClickListener,
     ClusterManager.IOnClusterItemInfoWindowClickListener
    {
        private readonly ClusteredMap map;
        private readonly ClusterManager clusterManager;
        private readonly ClusterLogic logic;

        public ClusterLogicListener(ClusteredMap map, ClusterManager clusterManager, ClusterLogic logic)
        {
            this.map = map;
            this.clusterManager = clusterManager;
            this.logic = logic;
        }

        public bool OnClusterClick(ICluster cluster)
        {
            map.SendClusterClicked(cluster.Items.Count,
                cluster.Items.Cast<ClusteredMarker>().Select(logic.LookupPin),
                new Position(cluster.Position.Latitude, cluster.Position.Longitude));

            return false;
        }

        public bool OnClusterItemClick(Java.Lang.Object nativeItemObj)
        {
            var targetPin = logic.LookupPin(nativeItemObj as ClusteredMarker);

            if (targetPin != null)
            {
                targetPin.SendTap();

                if (!ReferenceEquals(targetPin, map.SelectedPin))
                {
                    map.SelectedPin = targetPin;
                }

                map.SendPinClicked(targetPin);
            }

            return false;
        }

        public void OnClusterInfoWindowClick(ICluster cluster)
        {

        }

        public void OnClusterItemInfoWindowClick(Java.Lang.Object nativeItemObj)
        {
            var targetPin = logic.LookupPin(nativeItemObj as ClusteredMarker);

            if (targetPin != null)
            {
                targetPin.SendTap();
                map.SendInfoWindowClicked(targetPin);
            }
        }
    }
}
