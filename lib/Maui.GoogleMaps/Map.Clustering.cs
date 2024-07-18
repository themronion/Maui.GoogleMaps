using Maui.GoogleMaps.Clustering;

namespace Maui.GoogleMaps;

public partial class Map : View, IMap, IEnumerable<Pin>
{
    public static readonly BindableProperty ClusterOptionsProperty = BindableProperty.Create(nameof(ClusterOptionsProperty),
        typeof(ClusterOptions),
        typeof(Map),
        default(ClusterOptions));

    public event EventHandler<ClusterClickedEventArgs> ClusterClicked;

    internal Action OnCluster { get; set; }

    internal bool PendingClusterRequest { get; set; }

    public ClusterOptions ClusterOptions
    {
        get => (ClusterOptions)GetValue(ClusterOptionsProperty);
        set => SetValue(ClusterOptionsProperty, value);
    }

    public void Cluster()
    {
        SendCluster();
    }

    private void SendCluster()
    {
        if (OnCluster != null)
        {
            OnCluster.Invoke();
        }
        else
        {
            PendingClusterRequest = true;
        }
    }

    internal void SendClusterClicked(int itemsCount, IEnumerable<Pin> pins, Position position) 
        => ClusterClicked?.Invoke(this, new ClusterClickedEventArgs(itemsCount, pins, position));
}