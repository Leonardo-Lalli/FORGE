using System.Text.Json;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class SyncService
{
    private readonly DatabaseService db;
    private readonly PocketBaseService pb;
    private readonly ConnectivityService connectivity;

    public SyncService(DatabaseService db, PocketBaseService pb, ConnectivityService connectivity)
    {
        this.db = db;
        this.pb = pb;
        this.connectivity = connectivity;
    }

    public bool IsOnline => connectivity.IsOnline && pb.IsLoggedIn;

    public async Task SyncPendingWorkoutsAsync()
    {
        if (!IsOnline) return;

        var pending = await db.GetPendingSyncAsync();
        foreach (var w in pending)
        {
            try
            {
                var exercises = JsonSerializer.Deserialize<List<string>>(w.ExercisesJson) ?? new();
                var exerciseData = w.ExerciseDataJson;

                var payload = new Dictionary<string, object?>
                {
                    ["name"] = w.Name,
                    ["user_name"] = pb.CurrentUser?.Name ?? w.UserName,
                    ["date"] = w.Date,
                    ["exercises"] = exercises,
                    ["exercise_data"] = exerciseData,
                    ["volume"] = w.Volume,
                    ["duration"] = w.Duration,
                    ["notes"] = w.Notes
                };

                var (success, _) = await pb.CreateRecordAsync("logged_workouts", payload);
                if (success)
                    await db.MarkSyncedAsync(w.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Sync Pending] ex: {ex.Message}");
            }
        }
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
