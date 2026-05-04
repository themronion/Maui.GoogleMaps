using Microsoft.Extensions.DependencyInjection;

namespace Maui.GoogleMaps.Clustering.Hosting
{
    public static class AppHostBuilderExtensions
    {
        /// <summary>Registers clustering handlers and an optional <see cref="GoogleMapsClusteringOptions"/> instance (singleton).</summary>
        public static MauiAppBuilder UseGoogleMapsClustering(
            this MauiAppBuilder appBuilder,
            Action<GoogleMapsClusteringOptions>? configure = null)
        {
            var options = new GoogleMapsClusteringOptions();
            configure?.Invoke(options);
            appBuilder.Services.AddSingleton(options);

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
