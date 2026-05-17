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
                fonts.AddFont("Inter-Variable.ttf", "Inter");
                fonts.AddFont("Lexend-Variable.ttf", "Lexend");
                fonts.AddFont("SpaceGrotesk-Variable.ttf", "SpaceGrotesk");
            });

#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
#endif

        // Services
        builder.Services.AddSingleton<BuildSecrets>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<WorkoutSession>();

        builder.Services.AddSingleton(sp =>
        {
            var http = new HttpClient { BaseAddress = new Uri("https://pocketbase.server-casa-leo.duckdns.org/api/") };
            return new PocketBaseService(http, sp.GetRequiredService<BuildSecrets>());
        });
        builder.Services.AddSingleton(sp =>
        {
            var http = new HttpClient();
            return new ExerciseApiService(http, sp.GetRequiredService<BuildSecrets>(),
                sp.GetRequiredService<PocketBaseService>());
        });

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
        builder.Services.AddTransient<StartSessionViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<FriendRequestsViewModel>();

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
        builder.Services.AddTransient<StartSessionPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<FriendRequestsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
