namespace Forge.Services;

public class NotificationService(PocketBaseService pb)
{
#if !ANDROID
    private long restDeadline;
    private int pausedSeconds;
    private bool restPaused;
#endif
    public async Task<bool> EnableAsync()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33) && await Permissions.RequestAsync<Permissions.PostNotifications>() != PermissionStatus.Granted) return false;
#endif
        Preferences.Set("notifications_enabled", true);
        await pb.RegisterPushDeviceAsync();
        return true;
    }
    public void ScheduleRest(int seconds, string exerciseName = "", string imageUrl = "")
    {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.Start(seconds, exerciseName, imageUrl);
#else
        restPaused = false;
        pausedSeconds = 0;
        restDeadline = Environment.TickCount64 + Math.Max(0, seconds) * 1000L;
#endif
    }
    public async Task EnsureRestPermissionAsync()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33)) await Permissions.RequestAsync<Permissions.PostNotifications>();
#else
        await Task.CompletedTask;
#endif
    }
    public void CancelRest()
    {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.Cancel();
#else
        restDeadline = 0; restPaused = false; pausedSeconds = 0;
#endif
    }
    public int RestRemaining {
        get {
#if ANDROID
            return Forge.Platforms.Android.ForgeRestTimer.Remaining;
#else
            return restPaused ? pausedSeconds : Math.Max(0, (int)Math.Ceiling((restDeadline - Environment.TickCount64) / 1000.0));
#endif
        }
    }
    public bool IsRestPaused {
        get {
#if ANDROID
            return Forge.Platforms.Android.ForgeRestTimer.Paused;
#else
            return restPaused;
#endif
        }
    }
    public void ToggleRestPause() {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.TogglePause();
#else
        var remaining = RestRemaining;
        if (remaining <= 0) { CancelRest(); return; }
        restPaused = !restPaused;
        if (restPaused) pausedSeconds = remaining;
        else restDeadline = Environment.TickCount64 + remaining * 1000L;
#endif
    }
    public void OpenRestSoundSettings() {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.OpenSoundSettings();
#endif
    }
    public void RequestExactRestPermission() {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.RequestExactPermission();
#endif
    }
    public void FinishRest() {
#if ANDROID
        Forge.Platforms.Android.ForgeRestTimer.Finish();
#else
        CancelRest();
#endif
    }
    public void Show(string title, string body)
    {
#if ANDROID
        Forge.Platforms.Android.ForgeNotifications.Show(title, body);
#endif
    }
    public void SetReminder(bool enabled, TimeSpan time)
    {
        Preferences.Set("training_reminder", enabled);
        Preferences.Set("training_reminder_minutes", (int)time.TotalMinutes);
#if ANDROID
        Forge.Platforms.Android.ForgeNotifications.Cancel(101);
        if (enabled) {
            var next = DateTime.Now.Date + time;
            if (next <= DateTime.Now) next = next.AddDays(1);
            Forge.Platforms.Android.ForgeNotifications.Schedule(101, (int)(next - DateTime.Now).TotalSeconds, "Un momento per allenarti", "Apri FORGE e scegli la tua scheda.");
        }
#endif
    }
}
