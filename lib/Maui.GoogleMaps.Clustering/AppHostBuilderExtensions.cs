namespace Maui.GoogleMaps.Clustering.Hosting
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder UseGoogleMapsClustering(this MauiAppBuilder appBuilder)
        {
#if ANDROID || IOS
            appBuilder
                .ConfigureMauiHandlers(handlers =>
                    handlers.AddHandler<ClusteredMap, ClusterMapHandler>()
                );
#endif
            return appBuilder;
        }
    }
}
