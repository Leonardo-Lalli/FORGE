using System.Text.Json;
using Forge.Models;

namespace Forge.Services;
public record ExerciseProgressPoint(string Date, double Value, double Height);
public static class ExerciseProgress
{
    public static List<ExerciseProgressPoint> Build(IEnumerable<LocalWorkout> history, WorkoutExercise exercise, int metric)
    {
        var points = new List<(DateTime Date, double Value)>();
        foreach (var workout in history.Where(w => !w.IsDraft))
        {
            var date = AchievementEvaluator.ParseDate(workout.Date);
            if (!date.HasValue) continue;
            try
            {
                using var doc = JsonDocument.Parse(workout.ExerciseDataJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) continue;
                var sets = new List<(double Weight, int Reps)>();
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var id = item.TryGetProperty("exerciseId", out var eid) ? eid.GetString() : "";
                    var name = item.TryGetProperty("name", out var en) ? en.GetString() : "";
                    if (!((!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(exercise.ExerciseId)) ? id == exercise.ExerciseId : string.Equals(name?.Trim(), exercise.ExerciseName.Trim(), StringComparison.OrdinalIgnoreCase))) continue;
                    if (!item.TryGetProperty("sets", out var array) || array.ValueKind != JsonValueKind.Array) continue;
                    foreach (var s in array.EnumerateArray())
                    {
                        if (s.TryGetProperty("isCompleted", out var done) && done.ValueKind != JsonValueKind.True) continue;
                        if (s.TryGetProperty("weightKg", out var kg) && kg.TryGetDouble(out var weight) && double.IsFinite(weight) && weight >= 0 && s.TryGetProperty("reps", out var rp) && rp.TryGetInt32(out var reps) && reps > 0) sets.Add((weight, reps));
                    }
                }
                if (sets.Count == 0) continue;
                var value = metric switch { 1 => sets.Count, 2 => sets.Sum(s => s.Reps), 3 => sets.Sum(s => s.Weight * s.Reps), _ => sets.Max(s => s.Weight) };
                points.Add((date.Value, value));
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException) { }
        }
        var max = points.Select(p => p.Value).DefaultIfEmpty().Max();
        return points.OrderBy(p => p.Date).Select(p => new ExerciseProgressPoint(p.Date.ToString("dd/MM"), p.Value, max > 0 ? p.Value / max * 120 : 0)).ToList();
    }
}
