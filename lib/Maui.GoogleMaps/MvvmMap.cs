using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Maui.GoogleMaps;

public class MvvmMap : Map
{
    public MvvmMap()
    {
        BindableCircles = this.Circles;
        BindablePolygons = this.Polygons;
        BindablePolylines = this.Polylines;
        MoveToRegionAction = MoveToRegion;

        UiSettings.SetBinding(UiSettings.CompassEnabledEnabledProperty, new Binding(CompassEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.RotateGesturesEnabledProperty, new Binding(RotateGesturesEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.MyLocationButtonEnabledProperty, new Binding(MyLocationButtonEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.IndoorLevelPickerEnabledProperty, new Binding(IndoorLevelPickerEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.ScrollGesturesEnabledProperty, new Binding(ScrollGesturesEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.TiltGesturesEnabledProperty, new Binding(TiltGesturesEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.ZoomControlsEnabledProperty, new Binding(ZoomControlsEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.ZoomGesturesEnabledProperty, new Binding(ZoomGesturesEnabledProperty.PropertyName, BindingMode.OneWay, source: this));
        UiSettings.SetBinding(UiSettings.MapToolbarEnabledProperty, new Binding(MapToolbarEnabledProperty.PropertyName, BindingMode.OneWay, source: this));

        this.MapClicked += MvvmMap_MapClicked;
        this.InfoWindowClicked += MvvmMap_InfoWindowClicked;
    }

    private void MvvmMap_InfoWindowClicked(object sender, InfoWindowClickedEventArgs e)
    {
        if (InfoWindowClickedCommand?.CanExecute(e.Pin.BindingContext) is true) InfoWindowClickedCommand.Execute(e.Pin.BindingContext);
    }

    private void MvvmMap_MapClicked(object sender, MapClickedEventArgs e)
    {
        if (MapClickedCommand?.CanExecute(e.Point) is true) MapClickedCommand.Execute(e.Point);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == SelectedPinProperty.PropertyName)
        {
            SelectedItem = SelectedPin?.BindingContext;
        }
        else if (propertyName == SelectedItemProperty.PropertyName)
        {
            SelectedPin = Pins.FirstOrDefault(x => x.BindingContext == SelectedItem);
        }
    }

    #region SelectedItem
    public object SelectedItem
    {
        get { return (object)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(MvvmMap));
    #endregion

    #region MapClickedCommand
    public ICommand MapClickedCommand
    {
        get => (ICommand)GetValue(MapClickedCommandProperty);
        set => SetValue(MapClickedCommandProperty, value);
    }

    public static readonly BindableProperty MapClickedCommandProperty =
        BindableProperty.Create(
            nameof(MapClickedCommand),
            typeof(ICommand),
            typeof(MvvmMap));
    #endregion

    #region InfoWindowClickedCommand
    public ICommand InfoWindowClickedCommand
    {
        get => (ICommand)GetValue(InfoWindowClickedCommandProperty);
        set => SetValue(InfoWindowClickedCommandProperty, value);
    }

    public static readonly BindableProperty InfoWindowClickedCommandProperty =
        BindableProperty.Create(
            nameof(InfoWindowClickedCommand),
            typeof(ICommand),
            typeof(MvvmMap));
    #endregion

    #region BindableCircles
    public IList<Circle> BindableCircles
    {
        get => (IList<Circle>)GetValue(BindableCirclesProperty);
        set => SetValue(BindableCirclesProperty, value);
    }

    public static readonly BindableProperty BindableCirclesProperty =
        BindableProperty.Create(
            nameof(BindableCircles),
            typeof(IList<Circle>),
            typeof(MvvmMap),
            null,
            BindingMode.OneWayToSource);
    #endregion

    #region BindablePolygons
    public IList<Polygon> BindablePolygons
    {
        get => (IList<Polygon>)GetValue(BindablePolygonsProperty);
        set => SetValue(BindablePolygonsProperty, value);
    }

    public static readonly BindableProperty BindablePolygonsProperty =
        BindableProperty.Create(
            nameof(BindablePolygons),
            typeof(IList<Polygon>),
            typeof(MvvmMap),
            null,
            BindingMode.OneWayToSource);
    #endregion

    #region BindableCircles
    public IList<Polyline> BindablePolylines
    {
        get => (IList<Polyline>)GetValue(BindablePolylinesProperty);
        set => SetValue(BindablePolylinesProperty, value);
    }

    public static readonly BindableProperty BindablePolylinesProperty =
        BindableProperty.Create(
            nameof(BindablePolylines),
            typeof(IList<Polyline>),
            typeof(MvvmMap),
            null,
            BindingMode.OneWayToSource);
    #endregion

    #region MoveToRegionAction
    public Action<MapSpan, bool> MoveToRegionAction
    {
        get => (Action<MapSpan, bool>)GetValue(MoveToRegionActionProperty);
        set => SetValue(MoveToRegionActionProperty, value);
    }

    public static readonly BindableProperty MoveToRegionActionProperty =
        BindableProperty.Create(
            nameof(MoveToRegionAction),
            typeof(Action<MapSpan, bool>),
            typeof(MvvmMap),
            defaultBindingMode: BindingMode.OneWayToSource);
    #endregion

    #region CompassEnabled
    public bool CompassEnabled
    {
        get { return (bool)GetValue(CompassEnabledProperty); }
        set { SetValue(CompassEnabledProperty, value); }
    }

    public static readonly BindableProperty CompassEnabledProperty =
        BindableProperty.Create(
            nameof(CompassEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region RotateGesturesEnabled
    public bool RotateGesturesEnabled
    {
        get { return (bool)GetValue(RotateGesturesEnabledProperty); }
        set { SetValue(RotateGesturesEnabledProperty, value); }
    }

    public static readonly BindableProperty RotateGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(RotateGesturesEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region MyLocationButtonEnabled
    public bool MyLocationButtonEnabled
    {
        get { return (bool)GetValue(MyLocationButtonEnabledProperty); }
        set { SetValue(MyLocationButtonEnabledProperty, value); }
    }

    public static readonly BindableProperty MyLocationButtonEnabledProperty =
        BindableProperty.Create(
            nameof(MyLocationButtonEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region IndoorLevelPickerEnabled
    public bool IndoorLevelPickerEnabled
    {
        get { return (bool)GetValue(IndoorLevelPickerEnabledProperty); }
        set { SetValue(IndoorLevelPickerEnabledProperty, value); }
    }

    public static readonly BindableProperty IndoorLevelPickerEnabledProperty =
        BindableProperty.Create(
            nameof(IndoorLevelPickerEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region ScrollGesturesEnabled
    public bool ScrollGesturesEnabled
    {
        get { return (bool)GetValue(ScrollGesturesEnabledProperty); }
        set { SetValue(ScrollGesturesEnabledProperty, value); }
    }

    public static readonly BindableProperty ScrollGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(ScrollGesturesEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region TiltGesturesEnabled
    public bool TiltGesturesEnabled
    {
        get { return (bool)GetValue(TiltGesturesEnabledProperty); }
        set { SetValue(TiltGesturesEnabledProperty, value); }
    }

    public static readonly BindableProperty TiltGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(TiltGesturesEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region ZoomControlsEnabled
    public bool ZoomControlsEnabled
    {
        get { return (bool)GetValue(ZoomControlsEnabledProperty); }
        set { SetValue(ZoomControlsEnabledProperty, value); }
    }

    public static readonly BindableProperty ZoomControlsEnabledProperty =
        BindableProperty.Create(
            nameof(ZoomControlsEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region ZoomGesturesEnabled
    public bool ZoomGesturesEnabled
    {
        get { return (bool)GetValue(ZoomGesturesEnabledProperty); }
        set { SetValue(ZoomGesturesEnabledProperty, value); }
    }

    public static readonly BindableProperty ZoomGesturesEnabledProperty =
        BindableProperty.Create(
            nameof(ZoomGesturesEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region MaxZoomLevel
    public float MaxZoomLevel
    {
        get { return (float)GetValue(MaxZoomLevelProperty); }
        set { SetValue(MaxZoomLevelProperty, value); }
    }

    public static readonly BindableProperty MaxZoomLevelProperty =
        BindableProperty.Create(
            nameof(MaxZoomLevel),
            typeof(float),
            typeof(MvvmMap),
            21f);
    #endregion

    #region MinZoomLevel
    public float MinZoomLevel
    {
        get { return (float)GetValue(MinZoomLevelProperty); }
        set { SetValue(MinZoomLevelProperty, value); }
    }

    public static readonly BindableProperty MinZoomLevelProperty =
        BindableProperty.Create(
            nameof(MinZoomLevel),
            typeof(float),
            typeof(MvvmMap),
            3f);
    #endregion

    #region MapToolbarEnabled
    public bool MapToolbarEnabled
    {
        get { return (bool)GetValue(MapToolbarEnabledProperty); }
        set { SetValue(MapToolbarEnabledProperty, value); }
    }

    public static readonly BindableProperty MapToolbarEnabledProperty =
        BindableProperty.Create(
            nameof(MapToolbarEnabled),
            typeof(bool),
            typeof(MvvmMap),
            true);
    #endregion

    #region InfoWindowTemplate
    public DataTemplate InfoWindowTemplate
    {
        get { return (DataTemplate)GetValue(InfoWindowTemplateProperty); }
        set { SetValue(InfoWindowTemplateProperty, value); }
    }

    public static readonly BindableProperty InfoWindowTemplateProperty =
        BindableProperty.Create(
            nameof(InfoWindowTemplate),
            typeof(DataTemplate),
            typeof(MvvmMap));
    #endregion
}