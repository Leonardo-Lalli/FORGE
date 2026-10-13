using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace Forge.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class ForgeNotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        ForgeNotifications.Show(intent?.GetStringExtra("title") ?? "FORGE", intent?.GetStringExtra("body") ?? "");
        if (intent?.GetIntExtra("id", 0) == 101 && Preferences.Get("training_reminder", false))
            ForgeNotifications.Schedule(101, 86400, "Un momento per allenarti", "Apri FORGE e scegli la tua scheda.");
    }
}
public static class ForgeNotifications
{
    public const string Channel = "forge_activity";
    private static Context Context => global::Android.App.Application.Context;
    public static void Initialize()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
            ((NotificationManager?)Context.GetSystemService(Context.NotificationService))?.CreateNotificationChannel(new NotificationChannel(Channel, "Attività FORGE", NotificationImportance.Default));
    }
    public static void Show(string title, string body, string route = "")
    {
        if (!Preferences.Get("notifications_enabled", false)) return;
        var manager = (NotificationManager?)Context.GetSystemService(Context.NotificationService);
        Initialize();
        var notificationId = (int)(DateTime.UtcNow.Ticks % int.MaxValue);
        var intent = new Intent(Context, typeof(MainActivity));
        intent.PutExtra("forge_route", route);
        var pending = PendingIntent.GetActivity(Context, notificationId, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        var builder = new NotificationCompat.Builder(Context, Channel);
        builder.SetSmallIcon(Resource.Drawable.forge_icon_monochrome);
        builder.SetContentTitle(title); builder.SetContentText(body); builder.SetAutoCancel(true); builder.SetContentIntent(pending);
        var notification = builder.Build();
        manager?.Notify(notificationId, notification);
    }
    public static void Schedule(int id, int seconds, string title, string body)
    {
        Cancel(id);
        if (!Preferences.Get("notifications_enabled", false) || seconds <= 0) return;
        var intent = new Intent(Context, typeof(ForgeNotificationReceiver));
        intent.PutExtra("id", id); intent.PutExtra("title", title); intent.PutExtra("body", body);
        var pending = PendingIntent.GetBroadcast(Context, id, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        var alarms = (AlarmManager?)Context.GetSystemService(Context.AlarmService);
        var when = SystemClock.ElapsedRealtime() + seconds * 1000L;
        if (OperatingSystem.IsAndroidVersionAtLeast(23)) alarms?.SetAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, when, pending!);
        else alarms?.Set(AlarmType.ElapsedRealtimeWakeup, when, pending!);
    }
    public static void Cancel(int id)
    {
        var pending = PendingIntent.GetBroadcast(Context, id, new Intent(Context, typeof(ForgeNotificationReceiver)), PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable);
        if (pending != null) { ((AlarmManager?)Context.GetSystemService(Context.AlarmService))?.Cancel(pending); pending.Cancel(); }
    }
}
