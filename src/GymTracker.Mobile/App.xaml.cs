using GymTracker.Mobile.Services;
using GymTracker.Mobile.Views;

namespace GymTracker.Mobile;

public partial class App : Application
{
    private readonly ThemeService themeService;
    private readonly BuildSecrets buildSecrets;
    private readonly IServiceProvider services;

    public App(ThemeService themeService, BuildSecrets buildSecrets, IServiceProvider services)
    {
        InitializeComponent();
        this.themeService = themeService;
        this.buildSecrets = buildSecrets;
        this.services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        themeService.Initialize();

        var window = new Window(new ContentPage { BackgroundColor = Colors.Black });
        window.Created += async (_, _) =>
        {
            await InitializeAsync();
            var loginPage = services.GetRequiredService<LoginPage>();
            window.Page = loginPage;
        };
        return window;
    }

    private async Task InitializeAsync()
    {
        try { await buildSecrets.LoadAsync(); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[App] Secrets failed: {ex.Message}"); }

        var pb = services.GetRequiredService<PocketBaseService>();
        pb.Initialize();

        var sync = services.GetRequiredService<SyncService>();
        _ = sync.SyncPendingWorkoutsAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                System.Diagnostics.Debug.WriteLine($"[App Sync] ex: {t.Exception?.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
