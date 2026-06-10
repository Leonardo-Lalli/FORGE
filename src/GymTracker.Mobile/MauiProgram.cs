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
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<ConnectivityService>();

        builder.Services.AddHttpClient("pocketbase");
        builder.Services.AddHttpClient("exercisedbv1");
        builder.Services.AddHttpClient("wger");
        builder.Services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var secrets = sp.GetRequiredService<BuildSecrets>();
            return new PocketBaseService(factory, secrets);
        });
        builder.Services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var secrets = sp.GetRequiredService<BuildSecrets>();
            var db = sp.GetRequiredService<DatabaseService>();
            var pbService = sp.GetRequiredService<PocketBaseService>();
            return new ExerciseDbApiService(factory, db, pbService);
        });
        builder.Services.AddSingleton<SyncService>();
        builder.Services.AddSingleton<PlanService>();
        builder.Services.AddSingleton<CsvImportService>();
        builder.Services.AddSingleton<CsvExportService>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<ActiveWorkoutViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<StartSessionViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<FriendRequestsViewModel>();

        // Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<FeedPage>();
        builder.Services.AddTransient<StatsPage>();
        builder.Services.AddTransient<ActiveWorkoutPage>();
        builder.Services.AddTransient<ProfilePage>();
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
