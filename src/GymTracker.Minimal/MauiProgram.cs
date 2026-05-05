using GymTracker.Minimal.ViewModels;
using GymTracker.Minimal.Views;

namespace GymTracker.Minimal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<CatalogViewModel>();
        builder.Services.AddTransient<WorkoutViewModel>();
        builder.Services.AddTransient<SocialViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<CatalogPage>();
        builder.Services.AddTransient<WorkoutPage>();
        builder.Services.AddTransient<SocialPage>();
        builder.Services.AddTransient<ProfilePage>();

        return builder.Build();
    }
}
