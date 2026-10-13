using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

public static class PreviousPerformance
{
    public static List<(double Weight, int Reps)> Find(IEnumerable<LocalWorkout> history, WorkoutExercise exercise)
    {
        foreach (var workout in history.Where(w => !w.IsDraft).OrderByDescending(w => w.Date))
        {
            try
            {
                using var doc = JsonDocument.Parse(workout.ExerciseDataJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) continue;
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var id = item.TryGetProperty("exerciseId", out var idValue) ? idValue.GetString() : "";
                    var name = item.TryGetProperty("name", out var n) ? n.GetString() : "";
                    if (!(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(exercise.ExerciseId)
                        ? string.Equals(name?.Trim(), exercise.ExerciseName.Trim(), StringComparison.OrdinalIgnoreCase)
                        : id == exercise.ExerciseId)) continue;
                    var result = new List<(double, int)>();
                    if (!item.TryGetProperty("sets", out var sets) || sets.ValueKind != JsonValueKind.Array) continue;
                    foreach (var set in sets.EnumerateArray())
                    {
                        if (set.TryGetProperty("isCompleted", out var done) && done.ValueKind == JsonValueKind.False) continue;
                        if (set.TryGetProperty("weightKg", out var weight) && weight.TryGetDouble(out var kg) &&
                            set.TryGetProperty("reps", out var reps) && reps.TryGetInt32(out var r) && kg >= 0 && r > 0)
                            result.Add((kg, r));
                    }
                    if (result.Count > 0) return result;
                }
            }
            catch (JsonException) { }
        }
        return [];
    }

    public static void Apply(IEnumerable<LocalWorkout> history, WorkoutExercise exercise)
    {
        var previous = Find(history, exercise);
        for (var i = 0; i < exercise.Sets.Count && i < previous.Count; i++)
        {
            exercise.Sets[i].WeightKg = previous[i].Weight;
            exercise.Sets[i].Reps = previous[i].Reps;
            exercise.Sets[i].IsSuggested = true;
            exercise.Sets[i].PreviousText = $"{previous[i].Weight:0.#} × {previous[i].Reps}";
        }
    }
}
