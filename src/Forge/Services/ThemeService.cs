namespace Forge.Services;

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
        if (isInitialized) return;
        isInitialized = true;

        var isDark = Preferences.Get(ThemeKey, 1) == 1;
        var palette = isDark ? DarkPalette : LightPalette;
        WriteResources(palette);
    }

    public void Apply()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var app = Application.Current;
                if (app == null) return;

                var palette = IsDarkMode ? DarkPalette : LightPalette;
                WriteResources(palette);

                app.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThemeService] Apply failed: {ex.Message}");
            }
        });
    }

    private static void WriteResources(Dictionary<string, string> palette)
    {
        var resources = Application.Current?.Resources;
        if (resources == null) return;

        foreach (var kvp in palette)
        {
            resources[kvp.Key] = Color.FromArgb(kvp.Value);
        }
    }

    private static readonly Dictionary<string, string> LightPalette = new()
    {
        ["Surface"] = "#f8f9ff",
        ["SurfaceDim"] = "#ccdbf4",
        ["SurfaceBright"] = "#f8f9ff",
        ["SurfaceContainerLowest"] = "#ffffff",
        ["SurfaceContainerLow"] = "#eff4ff",
        ["SurfaceContainer"] = "#e6eeff",
        ["SurfaceContainerHigh"] = "#dde9ff",
        ["SurfaceContainerHighest"] = "#d5e3fd",
        ["SurfaceVariant"] = "#d5e3fd",

        ["OnSurface"] = "#0d1c2f",
        ["OnSurfaceVariant"] = "#434656",

        ["Primary"] = "#003ec7",
        ["OnPrimary"] = "#ffffff",
        ["PrimaryContainer"] = "#0052ff",
        ["OnPrimaryContainer"] = "#dfe3ff",

        ["Secondary"] = "#076e00",
        ["OnSecondary"] = "#ffffff",
        ["SecondaryContainer"] = "#18f900",
        ["OnSecondaryContainer"] = "#076d00",

        ["Tertiary"] = "#5c24c6",
        ["OnTertiary"] = "#ffffff",
        ["TertiaryContainer"] = "#7544df",

        ["Error"] = "#ba1a1a",
        ["OnError"] = "#ffffff",
        ["ErrorContainer"] = "#ffdad6",

        ["Outline"] = "#737688",
        ["OutlineVariant"] = "#c3c5d9",

        ["ElectricBlue"] = "#1a62ff",
        ["LimeGreen"] = "#076e00",
        ["AlertRed"] = "#ba1a1a",
        ["CardBackground"] = "#ffffff",
        ["PureBlack"] = "#000000",

        ["PageBackground"] = "#f8f9ff",
        ["TextPrimary"] = "#0d1c2f",
        ["TextSecondary"] = "#434656",
    };

    private static readonly Dictionary<string, string> DarkPalette = new()
    {
        ["Surface"] = "#131313",
        ["SurfaceDim"] = "#131313",
        ["SurfaceBright"] = "#3a3939",
        ["SurfaceContainerLowest"] = "#0e0e0e",
        ["SurfaceContainerLow"] = "#1c1b1b",
        ["SurfaceContainer"] = "#201f1f",
        ["SurfaceContainerHigh"] = "#2a2a2a",
        ["SurfaceContainerHighest"] = "#353534",
        ["SurfaceVariant"] = "#353534",

        ["OnSurface"] = "#e5e2e1",
        ["OnSurfaceVariant"] = "#bac9cc",

        ["Primary"] = "#c3f5ff",
        ["OnPrimary"] = "#00363d",
        ["PrimaryContainer"] = "#00e5ff",
        ["OnPrimaryContainer"] = "#00626e",

        ["Secondary"] = "#ffffff",
        ["OnSecondary"] = "#283500",
        ["SecondaryContainer"] = "#c3f400",
        ["OnSecondaryContainer"] = "#556d00",

        ["Tertiary"] = "#efeceb",
        ["OnTertiary"] = "#313030",
        ["TertiaryContainer"] = "#d2d0cf",

        ["Error"] = "#ffb4ab",
        ["OnError"] = "#690005",
        ["ErrorContainer"] = "#93000a",

        ["Outline"] = "#849396",
        ["OutlineVariant"] = "#3b494c",

        ["ElectricBlue"] = "#00daf3",
        ["LimeGreen"] = "#c3f400",
        ["AlertRed"] = "#ff3b30",
        ["CardBackground"] = "#1C1C1E",
        ["PureBlack"] = "#000000",

        ["PageBackground"] = "#131313",
        ["TextPrimary"] = "#e5e2e1",
        ["TextSecondary"] = "#bac9cc",
    };
}
