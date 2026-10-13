using Android.App;
using Android.Content;
using Android.OS;

namespace Forge.Platforms.Android;

[Service(Name = "com.leoforge.forge.RestTimerService", Exported = false)]
public class ForgeRestTimerService : Service
{
    private CancellationTokenSource? updates;
    private PowerManager.WakeLock? wake;
    public override IBinder? OnBind(Intent? intent) => null;
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        StartForeground(ForgeRestTimer.NotificationId, ForgeRestTimer.BuildCountdown());
        updates?.Cancel();
        updates = new();
        var token = updates.Token;
        _ = LoadThumbnailAsync(token);
        _ = UpdateAsync(token);
        return StartCommandResult.NotSticky;
    }
    private async Task UpdateAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (ForgeRestTimer.Remaining <= 0) { ForgeRestTimer.Finish(); StopSelf(); return; }
                if (ForgeRestTimer.Paused) { if (wake?.IsHeld == true) wake.Release(); }
                else if (wake?.IsHeld != true)
                {
                    wake ??= ((PowerManager)GetSystemService(PowerService)!).NewWakeLock(WakeLockFlags.Partial, "FORGE:RestTimer");
                    wake?.Acquire((ForgeRestTimer.Remaining + 10) * 1000L);
                }
                ForgeRestTimer.Publish();
                await Task.Delay(1000, token);
            }
        }
        catch (System.OperationCanceledException) { }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[RestTimer] {ex.GetType().Name}"); StopSelf(); }
    }
    private static async Task LoadThumbnailAsync(CancellationToken token)
    {
        var url = Preferences.Get("rest_image_url", "");
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != "https") return;
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5), MaxResponseContentBufferSize = 1024 * 1024 };
            var bytes = await client.GetByteArrayAsync(uri, token);
            using var options = new global::Android.Graphics.BitmapFactory.Options { InSampleSize = 4 };
            var bitmap = await global::Android.Graphics.BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length, options);
            if (token.IsCancellationRequested) { bitmap?.Dispose(); return; }
            await MainThread.InvokeOnMainThreadAsync(() => {
                if (token.IsCancellationRequested) { bitmap?.Dispose(); return; }
                var old = ForgeRestTimer.Thumbnail; ForgeRestTimer.Thumbnail = bitmap;
                ForgeRestTimer.Publish(); old?.Dispose();
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or Java.Lang.Exception) { System.Diagnostics.Debug.WriteLine($"[RestThumbnail] {ex.GetType().Name}"); }
    }
    public override void OnDestroy()
    {
        updates?.Cancel(); updates?.Dispose();
        if (wake?.IsHeld == true) wake.Release(); wake?.Dispose();
        var bitmap = ForgeRestTimer.Thumbnail; ForgeRestTimer.Thumbnail = null; bitmap?.Dispose();
        if (OperatingSystem.IsAndroidVersionAtLeast(24)) StopForeground(StopForegroundFlags.Remove);
#pragma warning disable CS0618
        else StopForeground(true);
#pragma warning restore CS0618
        base.OnDestroy();
    }
}
