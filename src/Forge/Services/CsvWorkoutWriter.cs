using System.Globalization;
using System.Text;
using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

public static class CsvWorkoutWriter
{
    public static string Write(IEnumerable<LocalWorkout> workouts, WeightUnitService units)
    {
        var text = new StringBuilder($"Date,WorkoutName,ExerciseName,Sets,Reps,{units.CsvHeader},Notes\n");
        foreach (var workout in workouts.Where(w => !w.IsDraft).OrderBy(w => w.Date))
        {
            var rows = new List<(string Name, double Kg, int Reps)>();
            try
            {
                using var data = JsonDocument.Parse(string.IsNullOrWhiteSpace(workout.ExerciseDataJson) ? "[]" : workout.ExerciseDataJson);
                if (data.RootElement.ValueKind == JsonValueKind.Array)
                    foreach (var exercise in data.RootElement.EnumerateArray())
                    {
                        if (!exercise.TryGetProperty("sets", out var sets) || sets.ValueKind != JsonValueKind.Array) continue;
                        var name = exercise.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                        foreach (var set in sets.EnumerateArray())
                        {
                            if (set.TryGetProperty("isCompleted", out var done) && done.ValueKind == JsonValueKind.False) continue;
                            if (!set.TryGetProperty("reps", out var r) || !r.TryGetInt32(out var reps) || reps <= 0) continue;
                            var kg = set.TryGetProperty("weightKg", out var weight) && weight.TryGetDouble(out var value) ? value : 0;
                            if (double.IsFinite(kg) && kg >= 0) rows.Add((name, kg, reps));
                        }
                    }
            }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException) { rows.Clear(); }
            if (rows.Count == 0)
            {
                // Older records may contain only exercise names; preserve their summary.
                List<string> names;
                try { names = JsonSerializer.Deserialize<List<string>>(workout.ExercisesJson) ?? []; }
                catch (JsonException) { names = []; }
                foreach (var name in names)
                    text.AppendLine($"{Quote(workout.Date)},{Quote(workout.Name)},{Quote(name)},0,0,0,{Quote(workout.Notes)}");
            }
            foreach (var row in rows)
                text.AppendLine($"{Quote(workout.Date)},{Quote(workout.Name)},{Quote(row.Name)},1,{row.Reps},{units.ToDisplay(row.Kg).ToString("0.########", CultureInfo.InvariantCulture)},{Quote(workout.Notes)}");
        }
        return text.ToString();
    }
    private static string Quote(string text) => "\"" + text.Replace("\"", "\"\"") + "\"";

    public static string[] SplitRows(string csv)
    {
        var rows = new List<string>();
        var quoted = false;
        var start = 0;
        for (var i = 0; i < csv.Length; i++)
        {
            if (csv[i] == '"')
            {
                if (quoted && i + 1 < csv.Length && csv[i + 1] == '"') i++;
                else quoted = !quoted;
            }
            else if (csv[i] == '\n' && !quoted)
            {
                var row = csv[start..i].Trim();
                if (row.Length > 0) rows.Add(row);
                start = i + 1;
            }
        }
        if (quoted) throw new FormatException("CSV con virgolette non chiuse.");
        if (csv[start..].Trim() is { Length: > 0 } last) rows.Add(last);
        return rows.ToArray();
    }

    public static string[] ParseRow(string line)
    {
        var fields = new List<string>();
        var text = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var value = line[i];
            if (value == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"') { text.Append('"'); i++; }
                else quoted = !quoted;
            }
            else if (value == ',' && !quoted) { fields.Add(text.ToString()); text.Clear(); }
            else text.Append(value);
        }
        if (quoted) throw new FormatException("CSV con virgolette non chiuse.");
        fields.Add(text.ToString());
        return fields.ToArray();
    }
}
