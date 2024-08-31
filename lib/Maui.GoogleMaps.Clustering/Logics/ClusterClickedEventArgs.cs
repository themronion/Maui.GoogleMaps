namespace Maui.GoogleMaps.Clustering.Logics
{
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
}
