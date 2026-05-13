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
        _ = InitializeAsync();
        return new Window(Handler?.MauiContext?.Services.GetService<Views.LoginPage>()
            ?? throw new InvalidOperationException("LoginPage non risolta"));
    }

    private async Task InitializeAsync()
    {
        await buildSecrets.LoadAsync();
        var pb = Handler?.MauiContext?.Services.GetService<PocketBaseService>();
        pb?.Initialize();
    }
}
