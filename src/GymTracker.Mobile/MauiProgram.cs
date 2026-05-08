using GymTracker.Mobile.Services;
using GymTracker.Mobile.ViewModels;
using GymTracker.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace GymTracker.Mobile;

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
                fonts.AddFont("Lexend-Bold.ttf", "LexendBold");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            });

        // Services
        builder.Services.AddSingleton<BuildSecrets>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<CatalogViewModel>();
        builder.Services.AddTransient<WorkoutViewModel>();
        builder.Services.AddTransient<ActiveWorkoutViewModel>();
        builder.Services.AddTransient<SocialViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();

        // Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<FeedPage>();
        builder.Services.AddTransient<StatsPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<CatalogPage>();
        builder.Services.AddTransient<WorkoutPage>();
        builder.Services.AddTransient<ActiveWorkoutPage>();
        builder.Services.AddTransient<SocialPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
