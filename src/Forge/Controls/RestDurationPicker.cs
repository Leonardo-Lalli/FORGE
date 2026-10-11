namespace Forge.Controls;

// Android renders native scrolling wheels; other targets retain native Picker controls.
public class RestDurationPicker : ContentView
{
    public static readonly BindableProperty SecondsProperty = BindableProperty.Create(
        nameof(Seconds), typeof(int), typeof(RestDurationPicker), 90, BindingMode.TwoWay,
        coerceValue: (_, value) => Math.Clamp((int)value, 0, 300), propertyChanged: OnSecondsChanged);
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(RestDurationPicker), Colors.Cyan,
        propertyChanged: (view, _, value) => ((RestDurationPicker)view).UpdateColor((Color)value));
    public int Seconds { get => (int)GetValue(SecondsProperty); set => SetValue(SecondsProperty, value); }
    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
    private readonly Picker minutes = new() { Title = "Minuti", ItemsSource = Enumerable.Range(0, 6).ToList() };
    private readonly Picker seconds = new() { Title = "Secondi", ItemsSource = Enumerable.Range(0, 60).ToList() };
    private bool updating;
    public RestDurationPicker()
    {
        var grid = new Grid { ColumnDefinitions = [new(GridLength.Star), new(GridLength.Star)], ColumnSpacing = 8 };
        grid.Add(minutes); grid.Add(seconds, 1);
        Content = grid;
        minutes.SelectedIndexChanged += (_, _) => UpdateValue();
        seconds.SelectedIndexChanged += (_, _) => UpdateValue();
        UpdatePickers(); UpdateColor(TextColor);
    }
    private static void OnSecondsChanged(BindableObject view, object oldValue, object value) => ((RestDurationPicker)view).UpdatePickers();
    private void UpdateValue()
    {
        if (updating || minutes.SelectedIndex < 0 || seconds.SelectedIndex < 0) return;
        Seconds = Math.Min(300, minutes.SelectedIndex * 60 + seconds.SelectedIndex);
        UpdatePickers();
    }
    private void UpdatePickers()
    {
        updating = true;
        minutes.SelectedIndex = Seconds / 60; seconds.SelectedIndex = Seconds % 60;
        seconds.IsEnabled = Seconds < 300;
        updating = false;
    }
    private void UpdateColor(Color color) { minutes.TextColor = color; seconds.TextColor = color; }
}
