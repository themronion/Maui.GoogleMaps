using Maui.GoogleMaps;
using Maui.GoogleMaps.Clustering.Logics;

namespace MauiGoogleMapSample;

public partial class ClusteringPage : ContentPage
{
    public ClusteringPage()
    {
        InitializeComponent();

        map.ClusterOptions.SetMinimumClusterSize(5);
        map.ClusterOptions.SetMaxDistanceBetweenClusteredItems(100); // android only

        //map.ClusterOptions.SetMinimumClusterSize(2);
        //map.ClusterOptions.SetMaxDistanceBetweenClusteredItems(60); // android only

        //map.ClusterClicked += Map_ClusterClicked;

        // New York pin
        buttonAddPins.Clicked += (sender, e) =>
        {
            map.Pins.Add(new Pin()
            {
                Type = PinType.Place,
                Label = "Igufaf",
                Position = new Position(36.6248851300317, 4.340352156036844),
                Tag = "id_0"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 1",
                Position = new Position(36.6348851300317, 4.350352156036844),
                Tag = "id_1"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 2",
                Position = new Position(36.6448851300317, 4.360352156036844),
                Tag = "id_2"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 3",
                Position = new Position(36.6548851300317, 4.370352156036844),
                Tag = "id_3"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 4",
                Position = new Position(36.6648851300317, 4.380352156036844),
                Tag = "id_4"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 5",
                Position = new Position(36.6748851300317, 4.390352156036844),
                Tag = "id_5"
            });

            map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Label = "Pin 6",
                Position = new Position(36.6248851300317, 4.440352156036844),
                Tag = "id_6"
            });


            map.MoveToRegion(MapSpan.FromCenterAndRadius(map.Pins.First().Position, Distance.FromMeters(20_000)));

            ((Button)sender).IsEnabled = false;
        };
    }

    private void Map_ClusterClicked(object sender, ClusterClickedEventArgs e)
    {
        DisplayAlert($"{e.Pins.Count()} pins:", string.Join("\n", e.Pins.Select(p => p.Label)), "Ok");
    }
}

