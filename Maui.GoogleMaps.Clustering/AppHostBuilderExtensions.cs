
using Maui.GoogleMaps.Handlers;
using Microsoft.Maui.LifecycleEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.GoogleMaps.Clustering.Hosting
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder UseGoogleMapsClustering(this MauiAppBuilder appBuilder)
        {
#if ANDROID            
            appBuilder
                .ConfigureMauiHandlers(handlers =>

                    handlers.AddHandler<ClusteredMap, ClusterMapHandler>()

                );
#endif
            return appBuilder;
        }
    }
}
