namespace GymTracker.Mobile.Services;

public class ThemeService
{
    private const string ThemeKey = "app_theme";

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
        var dark = Preferences.Get(ThemeKey, 1) == 1;
        if (dark)
            Apply(); // apply saved preference
        // else default dark from Colors.xaml is fine
    }

    public void Apply()
    {
        var app = Application.Current;
        if (app == null) return;

        var merged = app.Resources.MergedDictionaries;
        var darkSource = IsDarkMode
            ? "Resources/Styles/Colors.xaml"
            : "Resources/Styles/Colors.Light.xaml";

        // Remove existing color dictionaries
        var toRemove = merged
            .Where(d => d.Source != null &&
                   (d.Source.OriginalString.Contains("Colors.xaml") ||
                    d.Source.OriginalString.Contains("Colors.Light.xaml")))
            .ToList();

        foreach (var d in toRemove)
            merged.Remove(d);

        // Add the correct one
        merged.Add(new ResourceDictionary { Source = new Uri(darkSource, UriKind.Relative) });
    }
}
