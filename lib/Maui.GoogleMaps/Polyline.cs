using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Maui.GoogleMaps;

public sealed class Polyline : BindableObject
{
    public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(Polyline), 1f);
    public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Polyline), Colors.Blue);
    public static readonly BindableProperty IsClickableProperty = BindableProperty.Create(nameof(IsClickable), typeof(bool), typeof(Polyline), false);
    public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(int), typeof(Polyline), 0);
    public static readonly BindableProperty StrokePatternProperty = BindableProperty.Create(nameof(StrokePattern), typeof(LinePattern), typeof(Polyline), StrokePatternBuilder.SolidLine());

    private readonly ObservableCollection<Position> _positions = new ObservableCollection<Position>();

    private Action<Polyline, NotifyCollectionChangedEventArgs> _positionsChangedHandler = null;

    public float StrokeWidth
    {
        get { return (float)GetValue(StrokeWidthProperty); }
        set { SetValue(StrokeWidthProperty, value); }
    }

    public Color StrokeColor
    {
        get { return (Color)GetValue(StrokeColorProperty); }
        set { SetValue(StrokeColorProperty, value); }
    }

    public bool IsClickable
    {
        get { return (bool)GetValue(IsClickableProperty); }
        set { SetValue(IsClickableProperty, value); }
    }

    public int ZIndex
    {
        get { return (int)GetValue(ZIndexProperty); }
        set { SetValue(ZIndexProperty, value); }
    }

    public LinePattern StrokePattern
    {
        get { return (LinePattern)GetValue(StrokePatternProperty); }
        set { SetValue(StrokePatternProperty, value); }
    }

    public IList<Position> Positions
    {
        get { return _positions; }
    }

    public object Tag { get; set; }

    public object NativeObject { get; internal set; }

    public event EventHandler Clicked;

    public Polyline()
    {
    }

    internal bool SendTap()
    {
        EventHandler handler = Clicked;
        if (handler == null)
        {
            return false;
        }

        handler(this, EventArgs.Empty);
        return true;
    }

    internal void SetOnPositionsChanged(Action<Polyline, NotifyCollectionChangedEventArgs> handler)
    {
        _positionsChangedHandler = handler;
        if (handler != null)
        {
            _positions.CollectionChanged += OnCollectionChanged;
        }
        else
        {
            _positions.CollectionChanged -= OnCollectionChanged;
        }
    }

    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _positionsChangedHandler?.Invoke(this, e);
    }
}

public static class StrokePatternBuilder
{
    public static LinePattern SolidLine()
    {
        return null;
    }

    public static LinePattern DashedLine(int gapWidth = 10, int dashWidth = 20)
    {
        return new LinePattern { Type = LineTypes.Dashed, DashWidth = dashWidth, GapWidth = gapWidth };
    }

    public static LinePattern DottedLine(int gapWidth = 5, int dashWidth = 50)
    {
        return new LinePattern { Type = LineTypes.Dotted, GapWidth = gapWidth, DashWidth = dashWidth };
    }
}

public class LinePattern
{
    public int Type { get; set; }
    public int GapWidth { get; set; }
    public int DashWidth { get; set; }
}

public static class LineTypes
{
    public static int Straight = 0;
    public static int Dotted = 1;
    public static int Dashed = 2;
}

