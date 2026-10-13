using System.Text.Json;
using Forge.Models;
using Forge.Models.Dto;

namespace Forge.Services;
public static class WorkoutHistory
{
    public static async Task<List<LoggedWorkoutRecord>> LoadOwnAsync(PocketBaseService pb, DatabaseService db, bool localOnly = false, IEnumerable<LoggedWorkoutRecord>? cached = null)
    {
        var id = pb.CurrentUser?.Id;
        if (string.IsNullOrEmpty(id)) return [];
        var local = (await db.GetWorkoutsAsync(id)).Where(w => !w.IsDraft).ToList();
        List<LoggedWorkoutRecord> remote;
        try { remote = localOnly ? (cached ?? []).Where(w => w.User == id).ToList() : await pb.GetUserWorkoutsAsync(id); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException) { remote = []; }
        var result = remote.GroupBy(w => WorkoutSyncPayload.RecordId(w.Id)).ToDictionary(g => g.Key, g => g.First());
        foreach (var w in local.Where(w => w.PendingSync || !result.ContainsKey(WorkoutSyncPayload.RecordId(w.Id))))
            result[WorkoutSyncPayload.RecordId(w.Id)] = new LoggedWorkoutRecord { Id = w.Id, User = id, Name = w.Name, Date = w.Date, Duration = w.Duration, Volume = w.Volume, Notes = w.Notes,
                Exercises = JsonSerializer.Deserialize<List<string>>(w.ExercisesJson) ?? [], ExerciseData = w.ExerciseDataJson,
                Photos = JsonSerializer.Deserialize<List<string>>(w.PhotosJson) ?? [], Likes = w.Likes, UserName = w.UserName, IsLocal = true };
        return result.Values.OrderByDescending(w => w.Date).ToList();
    }
}
