using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

public static class WorkoutSyncPayload
{
    public static string RecordId(string localId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(localId);
        return localId.Length == 15 && localId.All(c => c is >= 'a' and <= 'z' or >= '0' and <= '9')
            ? localId : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(localId))).ToLowerInvariant()[..15];
    }

    public static Dictionary<string, object?> Create(LocalWorkout workout) => new()
    {
        ["id"] = RecordId(workout.Id), ["user"] = workout.UserId, ["user_name"] = workout.UserName,
        ["name"] = workout.Name, ["date"] = workout.Date, ["notes"] = workout.Notes,
        ["exercises"] = JsonSerializer.Deserialize<List<string>>(workout.ExercisesJson) ?? [],
        ["exercise_data"] = workout.ExerciseDataJson, ["volume"] = workout.Volume,
        ["duration"] = workout.Duration,
        ["photos"] = JsonSerializer.Deserialize<List<string>>(workout.PhotosJson) ?? []
    };
}
