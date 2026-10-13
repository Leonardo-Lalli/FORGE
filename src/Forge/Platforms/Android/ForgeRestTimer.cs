using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace Forge.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class ForgeRestTimerReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Action == "forge.rest.pause") ForgeRestTimer.TogglePause();
        else if (intent?.Action == "forge.rest.skip") ForgeRestTimer.Cancel();
        else ForgeRestTimer.Finish();
    }
}

// Android's notification chronometer runs independently of the app's UI thread.
public static class ForgeRestTimer
{
    public const int NotificationId = 100;
    private const string CountdownChannel = "forge_rest_countdown";
    private const string AlarmChannel = "forge_rest_finished_v2";
    private static Context Context => global::Android.App.Application.Context;
    private static AlarmManager? Alarms => (AlarmManager?)Context.GetSystemService(Context.AlarmService);
    public static bool Paused => Preferences.Get("rest_paused", false);
    public static int Remaining => Paused ? Preferences.Get("rest_paused_seconds", 0) :
        long.TryParse(Preferences.Get("rest_deadline", ""), out var end) ? Math.Max(0, (int)Math.Ceiling((end - SystemClock.ElapsedRealtime()) / 1000.0)) : 0;
    public static bool HasExactPermission => !OperatingSystem.IsAndroidVersionAtLeast(31) || Alarms?.CanScheduleExactAlarms() == true;

    public static void RequestExactPermission()
    {
        if (HasExactPermission) return;
        var intent = new Intent("android.settings.REQUEST_SCHEDULE_EXACT_ALARM", global::Android.Net.Uri.Parse("package:" + Context.PackageName));
        intent.AddFlags(ActivityFlags.NewTask);
        Context.StartActivity(intent);
    }
    private static PendingIntent AlarmIntent => PendingIntent.GetBroadcast(Context, 100,
        new Intent(Context, typeof(ForgeRestTimerReceiver)).SetAction("forge.rest.finish"), PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;

    public static void Start(int seconds, string exerciseName = "", string imageUrl = "")
    {
        Cancel();
        if (seconds <= 0) return;
        Preferences.Set("rest_deadline", (SystemClock.ElapsedRealtime() + seconds * 1000L).ToString());
        Preferences.Set("rest_duration", seconds);
        Preferences.Set("rest_exercise_name", exerciseName);
        Preferences.Set("rest_image_url", imageUrl);
        Preferences.Set("rest_paused", false);
        Schedule(seconds);
        Publish();
        var service = new Intent(Context, typeof(ForgeRestTimerService));
        if (OperatingSystem.IsAndroidVersionAtLeast(26)) Context.StartForegroundService(service);
        else Context.StartService(service);
    }
    private static void Schedule(int seconds)
    {
        var deadline = SystemClock.ElapsedRealtime() + seconds * 1000L;
        if (HasExactPermission) Alarms?.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, deadline, AlarmIntent);
        else Alarms?.SetAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, deadline, AlarmIntent);
    }
    public static void TogglePause()
    {
        var remaining = Remaining;
        if (remaining <= 0) { Cancel(); return; }
        Alarms?.Cancel(AlarmIntent);
        Preferences.Set("rest_paused", !Paused);
        if (Paused) Preferences.Set("rest_paused_seconds", remaining);
        else { Preferences.Set("rest_deadline", (SystemClock.ElapsedRealtime() + remaining * 1000L).ToString()); Schedule(remaining); }
        Publish();
    }
    public static void Cancel()
    {
        Context.StopService(new Intent(Context, typeof(ForgeRestTimerService)));
        Preferences.Remove("rest_deadline");
        Preferences.Set("rest_paused", false);
        Preferences.Set("rest_paused_seconds", 0);
        Alarms?.Cancel(AlarmIntent);
        ((NotificationManager?)Context.GetSystemService(Context.NotificationService))?.Cancel(NotificationId);
    }
    private static void Channels()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26)) return;
        var manager = (NotificationManager?)Context.GetSystemService(Context.NotificationService);
        var countdown = new NotificationChannel(CountdownChannel, "Recupero in corso", NotificationImportance.Low);
        countdown.SetSound(null, null);
        manager?.CreateNotificationChannel(countdown);
        var alarm = new NotificationChannel(AlarmChannel, "Fine recupero", NotificationImportance.High);
        alarm.EnableVibration(true);
        alarm.SetVibrationPattern([0, 300]);
        alarm.SetSound(SoundUri,
            new global::Android.Media.AudioAttributes.Builder().SetUsage(global::Android.Media.AudioUsageKind.Alarm)!.Build());
        manager?.CreateNotificationChannel(alarm);
    }
    private static NotificationCompat.Builder Builder(string channel)
    {
        Channels();
        var builder = new NotificationCompat.Builder(Context, channel);
        builder.SetSmallIcon(Resource.Drawable.forge_icon_monochrome);
        var open = new Intent(Context, typeof(MainActivity));
        builder.SetContentIntent(PendingIntent.GetActivity(Context, 100, open, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable));
        return builder;
    }
    private static global::Android.Net.Uri SoundUri => global::Android.Net.Uri.Parse("android.resource://" + Context.PackageName + "/raw/forge_rest_end")!;
    public static void OpenSoundSettings()
    {
        Channels();
        var intent = new Intent(OperatingSystem.IsAndroidVersionAtLeast(26) ? "android.settings.CHANNEL_NOTIFICATION_SETTINGS" : "android.settings.APPLICATION_DETAILS_SETTINGS");
        if (OperatingSystem.IsAndroidVersionAtLeast(26)) { intent.PutExtra("android.provider.extra.APP_PACKAGE", Context.PackageName); intent.PutExtra("android.provider.extra.CHANNEL_ID", AlarmChannel); }
        else intent.SetData(global::Android.Net.Uri.Parse("package:" + Context.PackageName));
        intent.AddFlags(ActivityFlags.NewTask); Context.StartActivity(intent);
    }
    public static global::Android.Graphics.Bitmap? Thumbnail { get; set; }
    public static Notification BuildCountdown()
    {
        var seconds = Remaining;
        var builder = Builder(CountdownChannel);
        builder.SetContentTitle(Paused ? "FORGE · Recupero in pausa" : "FORGE · Recupero");
        builder.SetContentText(Paused || !OperatingSystem.IsAndroidVersionAtLeast(24) ? $"{seconds / 60:00}:{seconds % 60:00}" : "Prossima serie al termine del timer");
        var exercise = Preferences.Get("rest_exercise_name", "");
        builder.SetContentText(string.IsNullOrWhiteSpace(exercise) ? "Prossima serie al termine del recupero" : exercise);
        builder.SetSubText($"{seconds / 60:00}:{seconds % 60:00} · {(Paused ? "In pausa" : "Prossima serie")}");
        builder.SetColor(global::Android.Graphics.Color.Rgb(0, 218, 243));
        var duration = Math.Max(1, Preferences.Get("rest_duration", 90));
        builder.SetProgress(duration, Math.Clamp(duration - seconds, 0, duration), false);
        if (Thumbnail != null) builder.SetLargeIcon(Thumbnail);
        builder.SetOngoing(true); builder.SetOnlyAlertOnce(true);
        builder.SetWhen(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + seconds * 1000L);
        builder.SetUsesChronometer(!Paused && OperatingSystem.IsAndroidVersionAtLeast(24));
        if (OperatingSystem.IsAndroidVersionAtLeast(24)) builder.SetChronometerCountDown(!Paused);
        foreach (var action in new[] { ("forge.rest.pause", Paused ? "Riprendi" : "Pausa"), ("forge.rest.skip", "Salta") })
            builder.AddAction(0, action.Item2, PendingIntent.GetBroadcast(Context, action.Item1 == "forge.rest.pause" ? 101 : 102,
                new Intent(Context, typeof(ForgeRestTimerReceiver)).SetAction(action.Item1), PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable));
        return builder.Build()!;
    }
    public static void Publish() => ((NotificationManager?)Context.GetSystemService(Context.NotificationService))?.Notify(NotificationId, BuildCountdown());
    public static void Finish()
    {
        // Ignore stale/replaced alarms; pause/skip never produce an end alert.
        if (Paused || !Preferences.ContainsKey("rest_deadline")) return;
        if (Remaining > 0) { Schedule(Remaining); return; }
        var exercise = Preferences.Get("rest_exercise_name", "");
        Cancel();
        var builder = Builder(AlarmChannel);
        builder.SetContentTitle("FORGE · Recupero terminato"); builder.SetContentText(string.IsNullOrWhiteSpace(exercise) ? "È il momento della prossima serie." : $"Prossima serie: {exercise}");
        builder.SetAutoCancel(true); builder.SetPriority(NotificationCompat.PriorityHigh);
        builder.SetDefaults((int)NotificationDefaults.Vibrate);
        builder.SetSound(SoundUri);
        builder.SetVibrate([0, 300]);
        ((NotificationManager?)Context.GetSystemService(Context.NotificationService))?.Notify(103, builder.Build());
    }
}
