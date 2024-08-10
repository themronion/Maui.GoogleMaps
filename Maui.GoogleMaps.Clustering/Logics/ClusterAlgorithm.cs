using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Maui.GoogleMaps.Clustering.Logics
{
    public enum ClusterAlgorithm
    {
        NonHierarchicalDistanceBased,
        GridBased,
        /// <summary>
        /// Android only
        /// </summary>
        VisibleNonHierarchicalDistanceBased
    }
   
}
