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

    public void Apply()
    {
        if (Application.Current == null) return;

        var source = IsDarkMode ? "Resources/Styles/Colors.Dark.xaml" : "Resources/Styles/Colors.Light.xaml";
        var merged = Application.Current.Resources.MergedDictionaries;

        var toRemove = merged.FirstOrDefault(d =>
            d.Source?.OriginalString?.Contains("Colors.Dark.xaml") == true ||
            d.Source?.OriginalString?.Contains("Colors.Light.xaml") == true);

        if (toRemove != null)
            merged.Remove(toRemove);

        merged.Add(new ResourceDictionary { Source = new Uri(source, UriKind.Relative) });
    }

    public void Initialize()
    {
        if (Preferences.ContainsKey(ThemeKey))
            Apply();
        // else keep the default (Colors.Dark.xaml loaded from App.xaml)
    }
}
