using System.Text.Json;
using Forge.Models;
namespace Forge.Services;
public static class PlanSessionChanges
{
    public static IReadOnlyList<string> Describe(WorkoutPlan original, string name, int rest, IEnumerable<WorkoutExercise> source)
    {
        var current = source.ToList();
        var changes = new List<string>();
        if (original.Name.Trim() != name.Trim()) changes.Add($"Nome della scheda modificato: «{original.Name}» → «{name.Trim()}»");
        if (original.RestSeconds != rest) changes.Add("Recupero generale modificato");
        foreach (var old in original.Exercises.Where(e => current.All(c => c.Id != e.Id)))
            changes.Add($"Esercizio rimosso: {old.ExerciseName}");
        foreach (var exercise in current)
        {
            var old = original.Exercises.FirstOrDefault(e => e.Id == exercise.Id);
            if (old == null) { changes.Add($"Esercizio aggiunto: {exercise.ExerciseName}"); continue; }
            var replaced = old.ExerciseId != exercise.ExerciseId || old.ExerciseName != exercise.ExerciseName;
            if (replaced) changes.Add($"Esercizio sostituito: «{old.ExerciseName}» → «{exercise.ExerciseName}»");
            if (old.RestSeconds != exercise.RestSeconds) changes.Add($"Recupero modificato: {exercise.ExerciseName}");
            var matched = old.Sets.Where(s => exercise.Sets.Any(c => c.Id == s.Id)).ToList();
            var stableIds = !replaced && matched.Count > 0;
            var added = stableIds ? exercise.Sets.Count(s => old.Sets.All(o => o.Id != s.Id)) : Math.Max(0, exercise.Sets.Count - old.Sets.Count);
            var removed = stableIds ? old.Sets.Count(s => exercise.Sets.All(c => c.Id != s.Id)) : Math.Max(0, old.Sets.Count - exercise.Sets.Count);
            if (added > 0) changes.Add($"{(added == 1 ? "Serie aggiunta" : $"{added} serie aggiunte")}: {exercise.ExerciseName}");
            if (removed > 0) changes.Add($"{(removed == 1 ? "Serie rimossa" : $"{removed} serie rimosse")}: {exercise.ExerciseName}");
            if (replaced) continue;
            var pairs = stableIds
                ? matched.Select(s => (Old: s, New: exercise.Sets.First(c => c.Id == s.Id))).ToList()
                : old.Sets.Zip(exercise.Sets, (a, b) => (Old: a, New: b)).ToList();
            if (pairs.Any(p => p.Old.WeightKg != p.New.WeightKg)) changes.Add($"Carichi modificati: {exercise.ExerciseName}");
            if (pairs.Any(p => p.Old.Reps != p.New.Reps)) changes.Add($"Ripetizioni modificate: {exercise.ExerciseName}");
            if (pairs.Any(p => p.Old.Kind != p.New.Kind)) changes.Add($"Tipo di serie modificato: {exercise.ExerciseName}");
        }
        var common = original.Exercises.Select(e => e.Id).Intersect(current.Select(e => e.Id)).ToHashSet();
        if (!original.Exercises.Where(e => common.Contains(e.Id)).Select(e => e.Id)
            .SequenceEqual(current.Where(e => common.Contains(e.Id)).Select(e => e.Id))) changes.Add("Ordine degli esercizi modificato");
        return changes;
    }

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
