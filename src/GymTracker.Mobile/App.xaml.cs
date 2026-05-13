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
        var window = new Window(new AppShell());
        window.Created += async (_, _) => await OnWindowCreatedAsync();
        return window;
    }

    private async Task OnWindowCreatedAsync()
    {
        try
        {
            await buildSecrets.LoadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] BuildSecrets failed: {ex.Message}");
        }

        if (Handler?.MauiContext?.Services.GetService<PocketBaseService>() is { } pb)
        {
            pb.Initialize();

            var loggedIn = await pb.TryAutoLoginAsync();
            if (!loggedIn)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("login");
                });
            }
        }
    }
}
