using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Models;
using Forge.Models.Dto;

namespace Forge.Services;

public class SyncService
{
    private readonly DatabaseService db;
    private readonly PocketBaseService pb;
    private readonly ConnectivityService connectivity;
    private readonly SemaphoreSlim syncGate = new(1, 1);

    public SyncService(DatabaseService db, PocketBaseService pb, ConnectivityService connectivity)
    {
        this.db = db;
        this.pb = pb;
        this.connectivity = connectivity;
        connectivity.ConnectivityChanged += (_, online) =>
        {
            if (online) _ = SyncPendingWorkoutsAsync().ContinueWith(t =>
                System.Diagnostics.Debug.WriteLine($"[Sync] {t.Exception?.GetType().Name}"),
                TaskContinuationOptions.OnlyOnFaulted);
        };
    }

    public bool IsOnline => connectivity.IsOnline && pb.IsLoggedIn;

    public async Task SyncPendingWorkoutsAsync()
    {
        if (!IsOnline || !await syncGate.WaitAsync(0)) return;
        try
        {
            var userId = pb.CurrentUser?.Id;
            var pending = await db.GetPendingSyncAsync();
            foreach (var w in pending.Where(w => w.UserId == userId && !w.IsDraft))
            {
                if (!IsOnline || pb.CurrentUser?.Id != userId) break;
                try
                {
                    if (await pb.UploadWorkoutAsync(w))
                    {
                        await db.MarkSyncedAsync(w.Id, WorkoutSyncPayload.RecordId(w.Id));
                        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new Forge.Messages.WorkoutSavedMessage());
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Sync Pending] {ex.GetType().Name}"); }
            }
        }
        finally { syncGate.Release(); }
    }

    public async Task SyncWorkoutsFromServerAsync()
    {
        if (!IsOnline) return;

        try
        {
            var workouts = await pb.GetMyWorkoutsAsync(100);
            foreach (var w in workouts)
            {
                var local = new LocalWorkout
                {
                    Id = w.Id,
                    UserId = w.User,
                    Name = w.Name,
                    Date = w.Date,
                    ExercisesJson = JsonSerializer.Serialize(w.Exercises),
                    Volume = w.Volume,
                    Duration = w.Duration,
                    Notes = w.Notes,
                    ExerciseDataJson = w.ExerciseData,
                    PhotosJson = JsonSerializer.Serialize(w.Photos),
                    Likes = w.Likes,
                    LikedByJson = JsonSerializer.Serialize(w.LikedBy),
                    UserName = w.UserName,
                    PendingSync = false,
                    LastSynced = DateTime.UtcNow
                };
                await db.SaveWorkoutAsync(local);
            }

            var followedWorkouts = await pb.GetFollowedWorkoutsAsync();
            foreach (var w in followedWorkouts)
            {
                var local = new LocalWorkout
                {
                    Id = w.Id,
                    UserId = w.User,
                    Name = w.Name,
                    Date = w.Date,
                    ExercisesJson = JsonSerializer.Serialize(w.Exercises),
                    Volume = w.Volume,
                    Duration = w.Duration,
                    Notes = w.Notes,
                    ExerciseDataJson = w.ExerciseData,
                    PhotosJson = JsonSerializer.Serialize(w.Photos),
                    Likes = w.Likes,
                    LikedByJson = JsonSerializer.Serialize(w.LikedBy),
                    UserName = w.UserName,
                    PendingSync = false,
                    LastSynced = DateTime.UtcNow
                };
                await db.SaveWorkoutAsync(local);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Sync Server] ex: {ex.Message}");
        }
    }

    public async Task SyncExerciseCacheAsync()
    {
        if (!IsOnline) return;

        try
        {
            var cached = await db.GetCachedExercisesAsync();
            foreach (var ex in cached)
            {
                if (!pb.IsLoggedIn) break;

                var payload = new Dictionary<string, object?>
                {
                    ["name"] = ex.Name,
                    ["bodyPart"] = ex.BodyPart,
                    ["equipment"] = ex.Equipment,
                    ["instructions"] = JsonSerializer.Deserialize<List<string>>(ex.InstructionsJson) ?? new(),
                    ["imageUrl"] = ex.ImageUrl,
                    ["category"] = ex.Category,
                    ["level"] = ex.Level,
                    ["force"] = ex.Force,
                    ["mechanic"] = ex.Mechanic
                };
                await pb.CreateRecordAsync("excercise", payload);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Sync ExercCache] ex: {ex.Message}");
        }
    }
}
