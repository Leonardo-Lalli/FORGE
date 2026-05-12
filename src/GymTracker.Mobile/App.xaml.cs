using GymTracker.Mobile.Services;

namespace GymTracker.Mobile;

public partial class App : Application
{
    private readonly ThemeService themeService;
    private readonly BuildSecrets buildSecrets;

    public App(ThemeService themeService, BuildSecrets buildSecrets)
    {
        InitializeComponent();
        this.themeService = themeService;
        this.buildSecrets = buildSecrets;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        themeService.Initialize();
        _ = buildSecrets.LoadAsync();
        return new Window(new AppShell());
    }
}
