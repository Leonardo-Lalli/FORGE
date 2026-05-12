namespace GymTracker.Mobile.Services;

public class ThemeService
{
    private const string ThemeKey = "app_theme";
    private bool isInitialized;

    public bool IsDarkMode
    {
        get => Preferences.Get(ThemeKey, 1) == 1;
        set
        {
            Preferences.Set(ThemeKey, value ? 1 : 0);
            Apply();
        }
    }

    public void Initialize()
    {
        if (isInitialized)
            return;

        isInitialized = true;
        var dark = Preferences.Get(ThemeKey, 1) == 1;
        if (dark)
            return;

        Apply();
    }

    public void Apply()
    {
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(Apply);
            return;
        }

        var app = Application.Current;
        if (app == null) return;

        try
        {
            var merged = app.Resources.MergedDictionaries;
            var darkSource = IsDarkMode
                ? "Resources/Styles/Colors.xaml"
                : "Resources/Styles/Colors.Light.xaml";

            var toRemove = merged
                .Where(d => d.Source != null &&
                       (d.Source.OriginalString.Contains("Colors.xaml") ||
                        d.Source.OriginalString.Contains("Colors.Light.xaml")))
                .ToList();

            foreach (var d in toRemove)
                merged.Remove(d);

            merged.Add(new ResourceDictionary { Source = new Uri(darkSource, UriKind.Relative) });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ThemeService] Apply failed: {ex.Message}");
        }
    }
}
