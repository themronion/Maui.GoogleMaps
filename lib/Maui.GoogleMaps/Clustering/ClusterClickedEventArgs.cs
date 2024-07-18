// Original code from https://github.com/mierzynskim/Xamarin.Forms.GoogleMaps.Clustering/

namespace Maui.GoogleMaps.Clustering;

public sealed class ClusterClickedEventArgs : EventArgs
{
    public int ItemsCount { get; }

    public IEnumerable<Pin> Pins { get; }

    public Position Position { get; }

    internal ClusterClickedEventArgs(int itemsCount, IEnumerable<Pin> pins, Position position)
    {
        ItemsCount = itemsCount;
        Pins = pins;
        Position = position;
    }
}