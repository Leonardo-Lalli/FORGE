#if ANDROID
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Forge.Controls;

// Use the full available width: NumberPicker's intrinsic width otherwise compresses
// both weighted wheels to the left while the MAUI headings span the whole panel.
internal class RestWheelsLayout(global::Android.Content.Context context) : LinearLayout(context)
{
    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        var width = global::Android.Views.View.MeasureSpec.GetSize(widthMeasureSpec);
        if (width > 0 && global::Android.Views.View.MeasureSpec.GetMode(widthMeasureSpec) != global::Android.Views.MeasureSpecMode.Unspecified)
            widthMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(width, global::Android.Views.MeasureSpecMode.Exactly);
        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
    }
}

public class RestDurationPickerHandler : ViewHandler<RestDurationPicker, LinearLayout>
{
    private NumberPicker minutes = null!;
    private NumberPicker seconds = null!;
    private bool updating;
    public static readonly IPropertyMapper<RestDurationPicker, RestDurationPickerHandler> RestMapper =
        new PropertyMapper<RestDurationPicker, RestDurationPickerHandler>(ViewMapper)
        {
            [nameof(RestDurationPicker.Seconds)] = (handler, _) => handler.UpdateWheels(),
            [nameof(RestDurationPicker.TextColor)] = (handler, _) => handler.UpdateWheels()
        };
    public RestDurationPickerHandler() : base(RestMapper) { }
    protected override LinearLayout CreatePlatformView()
    {
        var layout = new RestWheelsLayout(Context) { Orientation = Orientation.Horizontal };
        minutes = new NumberPicker(Context) { MinValue = 0, MaxValue = 5, WrapSelectorWheel = false };
        seconds = new NumberPicker(Context) { MinValue = 0, MaxValue = 59, WrapSelectorWheel = false };
        seconds.SetDisplayedValues(Enumerable.Range(0, 60).Select(n => n.ToString("00")).ToArray());
        layout.AddView(minutes, new LinearLayout.LayoutParams(0, -1, 1));
        layout.AddView(seconds, new LinearLayout.LayoutParams(0, -1, 1));
        minutes.ValueChanged += OnValueChanged; seconds.ValueChanged += OnValueChanged;
        return layout;
    }
    private void OnValueChanged(object? sender, NumberPicker.ValueChangeEventArgs args)
    {
        if (updating) return;
        VirtualView.Seconds = Math.Min(300, minutes.Value * 60 + seconds.Value);
        UpdateWheels();
    }
    private void UpdateWheels()
    {
        if (minutes == null) return;
        updating = true;
        minutes.Value = VirtualView.Seconds / 60; seconds.Value = VirtualView.Seconds % 60;
        seconds.Enabled = VirtualView.Seconds < 300;
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            minutes.SetTextColor(VirtualView.TextColor.ToPlatform());
            seconds.SetTextColor(VirtualView.TextColor.ToPlatform());
        }
        updating = false;
    }
    protected override void DisconnectHandler(LinearLayout platformView)
    {
        minutes.ValueChanged -= OnValueChanged; seconds.ValueChanged -= OnValueChanged;
        base.DisconnectHandler(platformView);
    }
}
#endif
