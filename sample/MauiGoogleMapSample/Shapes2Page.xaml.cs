using Maui.GoogleMaps;

namespace MauiGoogleMapSample
{
    public partial class Shapes2Page : ContentPage
    {
        public Shapes2Page()
        {
            InitializeComponent();

            pickerZIndex.Items.Add("Red");
            pickerZIndex.Items.Add("Yellow");
            pickerZIndex.Items.Add("Green");

            // Add Polygons
            var polygon1 = new Polygon();
            polygon1.Positions.Add(new Position(35.65, 139.83));
            polygon1.Positions.Add(new Position(35.75, 139.93));
            polygon1.Positions.Add(new Position(35.85, 139.83));
            polygon1.Positions.Add(new Position(35.65, 139.83));
            polygon1.StrokeWidth = 5f;
            polygon1.StrokeColor = Colors.Red;
            polygon1.FillColor = Color.FromRgba(255, 0, 0, 160);
            map.Polygons.Add(polygon1);
            map.Polygons.Add(CreateShiftedPolygon(polygon1, 0d, 0.05d, Colors.Yellow));
            map.Polygons.Add(CreateShiftedPolygon(polygon1, 0d, 0.10d, Colors.Green));

            // Add Polylines
            var polyline1 = new Polyline();
            polyline1.StrokeWidth = 10f;
            polyline1.StrokeColor = Colors.Red;
            polyline1.Positions.Add(new Position(36.00, 139.83));
            polyline1.Positions.Add(new Position(36.10, 139.93));
            polyline1.Positions.Add(new Position(36.00, 140.03));
            map.Polylines.Add(polyline1);
            map.Polylines.Add(CreateShiftedPolyline(polyline1, 0d, 0.05d, Colors.Yellow));
            map.Polylines.Add(CreateShiftedPolyline(polyline1, 0d, 0.10d, Colors.Green));

            //Add Dashed Polyline
            var polylineDashed = new Polyline();
            polylineDashed.StrokeWidth = 10f;
            polylineDashed.StrokeColor = Colors.Green;
            polylineDashed.Positions.Add(new Position(36.20, 139.83));
            polylineDashed.Positions.Add(new Position(36.30, 139.93));
            polylineDashed.Positions.Add(new Position(36.40, 140.03));
            polylineDashed.StrokePattern = StrokePatternBuilder.DashedLine();
            map.Polylines.Add(polylineDashed);

            //Add Dotted Polyline (appears as dashed on iOS)
            var polylineDotted = new Polyline();
            polylineDotted.StrokeWidth = 10f;
            polylineDotted.StrokeColor = Colors.Orange;
            polylineDotted.Positions.Add(new Position(36.50, 139.83));
            polylineDotted.Positions.Add(new Position(36.60, 139.93));
            polylineDotted.Positions.Add(new Position(36.70, 140.03));
            polylineDotted.StrokePattern = StrokePatternBuilder.DottedLine();
            map.Polylines.Add(polylineDotted);

            // Add Circles
            var circle1 = new Circle();
            circle1.StrokeWidth = 10f;
            circle1.StrokeColor = Colors.Red;
            circle1.FillColor = Color.FromRgba(255, 0, 0, 160);
            circle1.Center = new Position(35.85, 140.23);
            circle1.Radius = Distance.FromKilometers(8);
            map.Circles.Add(circle1);
            map.Circles.Add(CreateShiftedCircle(circle1, 0d, 0.05d, Colors.Yellow));
            map.Circles.Add(CreateShiftedCircle(circle1, 0d, 0.10d, Colors.Green));

            // Fit to all shapes
            var bounds = Maui.GoogleMaps.Bounds.FromPositions(map.Polygons.SelectMany(poly => poly.Positions));
            bounds = bounds.Including(Maui.GoogleMaps.Bounds.FromPositions(map.Polylines.SelectMany(poly => poly.Positions)));
            bounds = bounds.Including(Maui.GoogleMaps.Bounds.FromPositions(map.Circles.Select(cir => cir.Center)));

            // Move to Front
            pickerZIndex.SelectedIndexChanged += (sender, e) =>
            {
                var i = pickerZIndex.SelectedIndex;
                map.Polylines[i].ZIndex = map.Polylines.Max(p => p.ZIndex) + 1;
                map.Polygons[i].ZIndex = map.Polygons.Max(p => p.ZIndex) + 1;
                map.Circles[i].ZIndex = map.Circles.Max(p => p.ZIndex) + 1;
            };
            pickerZIndex.SelectedIndex = 0;

            map.InitialCameraUpdate = CameraUpdateFactory.NewBounds(bounds, 5);
        }

        private Polygon CreateShiftedPolygon(Polygon polygon, double shiftLat, double shiftLon, Color color)
        {
            var poly = new Polygon();
            poly.StrokeWidth = polygon.StrokeWidth;
            poly.StrokeColor = color;
            poly.FillColor = color;

            foreach (var p in polygon.Positions)
            {
                poly.Positions.Add(new Position(p.Latitude + shiftLat, p.Longitude + shiftLon));
            }

            return poly;
        }

        private Polyline CreateShiftedPolyline(Polyline polyline, double shiftLat, double shiftLon, Color color)
        {
            var poly = new Polyline();
            poly.StrokeWidth = polyline.StrokeWidth;
            poly.StrokeColor = color;
            foreach (var p in polyline.Positions)
            {
                poly.Positions.Add(new Position(p.Latitude + shiftLat, p.Longitude + shiftLon));
            }

            return poly;
        }

        private Circle CreateShiftedCircle(Circle circle, double shiftLat, double shiftLon, Color color)
        {
            var cir = new Circle();
            cir.StrokeWidth = circle.StrokeWidth;
            cir.StrokeColor = color;
            cir.FillColor = color;
            cir.Radius = circle.Radius;
            cir.Center = new Position(circle.Center.Latitude + shiftLat, circle.Center.Longitude + shiftLon);

            return cir;
        }
    }
}
