using GymTracker.Mobile.Services;

namespace GymTracker.Mobile;

public partial class App : Application
{
    private readonly ThemeService themeService;

    public App(ThemeService themeService)
    {
        InitializeComponent();
        this.themeService = themeService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        themeService.Initialize();
        return new Window(new AppShell());
    }
}