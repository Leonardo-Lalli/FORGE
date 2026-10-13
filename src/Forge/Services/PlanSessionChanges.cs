using System.Text.Json;
using Forge.Models;
namespace Forge.Services;
public static class PlanSessionChanges
{
    public static string Signature(string name, string notes, int rest, IEnumerable<WorkoutExercise> exercises) => JsonSerializer.Serialize(new {
        name = name.Trim(), rest,
        exercises = exercises.Select(e => new { e.Id, e.ExerciseId, e.ExerciseName, e.RestSeconds,
            sets = e.Sets.Select(s => new { s.WeightKg, s.Reps, s.Kind }).ToList() }).ToList()
    });
    public readonly record struct ExerciseNote(string Id, string ExerciseId, string Name, string Notes);

    // Match the entry and exercise identity: a replacement must not change the old exercise's notes.
    public static bool ApplyNotes(WorkoutPlan plan, string notes, IReadOnlyList<ExerciseNote> exerciseNotes)
    {
        var changed = plan.Notes != notes;
        plan.Notes = notes;
        foreach (var exercise in plan.Exercises)
        {
            var match = exerciseNotes.FirstOrDefault(n => n.Id == exercise.Id &&
                n.ExerciseId == exercise.ExerciseId && n.Name == exercise.ExerciseName);
            if (match.Id == null || exercise.Notes == match.Notes) continue;
            exercise.Notes = match.Notes;
            changed = true;
        }
        return changed;
    }

    public static List<WorkoutExercise> CopyExercises(IEnumerable<WorkoutExercise> exercises)
    {
        var copy = JsonSerializer.Deserialize<List<WorkoutExercise>>(JsonSerializer.Serialize(exercises.ToList())) ?? [];
        foreach (var ex in copy) { ex.IsCompleted = false; ex.ExecutionTipIndex = 0; foreach (var set in ex.Sets) { set.IsCompleted = false; set.IsSuggested = false; set.PreviousText = "—"; } }
        return copy;
    }
}
