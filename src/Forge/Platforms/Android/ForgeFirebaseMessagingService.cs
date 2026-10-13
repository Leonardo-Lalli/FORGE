using Android.App;
using Android.Content;
using Firebase.Messaging;
using Forge.Services;

namespace Forge.Platforms.Android;
[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class ForgeFirebaseMessagingService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        _ = SaveTokenAsync(token);
    }
    internal static async Task SaveTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync("forge_push_token", token);
            var pb = IPlatformApplication.Current?.Services.GetService<PocketBaseService>();
            if (pb != null) await pb.RegisterPushDeviceAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { System.Diagnostics.Debug.WriteLine("[Push] Registrazione rimandata."); }
    }
    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        var notification = message.GetNotification();
        message.Data.TryGetValue("route", out var route);
        ForgeNotifications.Show(notification?.Title ?? "FORGE", notification?.Body ?? "Nuova attività", route ?? "");
    }
}
