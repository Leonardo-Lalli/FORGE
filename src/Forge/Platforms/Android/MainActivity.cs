using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Forge;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = global::Android.Views.SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Forge.Platforms.Android.ForgeNotifications.Initialize();
        if (Firebase.FirebaseApp.InitializeApp(this) != null) Firebase.Messaging.FirebaseMessaging.Instance.GetToken().AddOnSuccessListener(this);
        QueueRoute(Intent);
    }
    public void OnSuccess(Java.Lang.Object? result) { if (result != null) _ = Forge.Platforms.Android.ForgeFirebaseMessagingService.SaveTokenAsync(result.ToString()); }
    protected override void OnNewIntent(Android.Content.Intent? intent) { base.OnNewIntent(intent); QueueRoute(intent); }
    private static void QueueRoute(Android.Content.Intent? intent)
    {
        var route = intent?.GetStringExtra("forge_route") ?? intent?.GetStringExtra("route");
        if (!string.IsNullOrEmpty(route))
        {
            Preferences.Set("pending_notification_route", route);
            MainThread.BeginInvokeOnMainThread(async () => await Forge.Services.NotificationNavigation.OpenPendingAsync());
        }
    }
}
