using Forge.Models;

namespace Forge.Services;

public class CsvImportService
{
    private readonly DatabaseService db;
    private readonly ExerciseDbApiService exerciseDbApi;
    private readonly PocketBaseService pb;

    public CsvImportService(DatabaseService db, ExerciseDbApiService exerciseDbApi, PocketBaseService pb)
    {
        this.db = db;
        this.exerciseDbApi = exerciseDbApi;
        this.pb = pb;
    }

    private const int MaxCsvBytes = 2 * 1024 * 1024; // 2MB
    private const int MaxCsvRows = 1000;

    public async Task<(int Imported, int Skipped, List<string> Errors)> ImportFromCsvAsync(string csvContent, string userId, string userName)
    {
        var imported = 0;
        var skipped = 0;
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(csvContent) || csvContent.Length > MaxCsvBytes)
        {
            errors.Add(csvContent.Length > MaxCsvBytes
                ? $"CSV troppo grande (max {MaxCsvBytes / 1024 / 1024}MB)."
                : "CSV vuoto.");
            return (0, 0, errors);
        }

        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2)
        {
            errors.Add("CSV vuoto o con sola intestazione.");
            return (0, lines.Length, errors);
        }
        if (lines.Length > MaxCsvRows)
        {
            errors.Add($"CSV contiene {lines.Length} righe (max {MaxCsvRows}).");
            return (0, 0, errors);
        }

        var header = lines[0].ToLowerInvariant().Split(',');
        var dateIdx = IndexOf(header, "date", "data");
        var nameIdx = IndexOf(header, "workoutname", "nomeallenamento", "workout");
        var exIdx = IndexOf(header, "exercisename", "esercizio", "exercise");
        var setsIdx = IndexOf(header, "sets", "serie");
        var repsIdx = IndexOf(header, "reps", "ripetizioni", "repetitions");
        var weightIdx = IndexOf(header, "weightkg", "pesokg", "peso", "weight");
        var notesIdx = IndexOf(header, "notes", "note");

        if (dateIdx < 0 || exIdx < 0)
        {
            errors.Add("Colonne obbligatorie mancanti: Date, ExerciseName.");
            return (0, 0, errors);
        }

        string? currentWorkoutName = null;
        string? currentDate = null;
        string? currentNotes = null;
        var currentExercises = new List<string>();
        var currentExerciseData = new List<object>();

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = ParseCsvLine(lines[i]);
            if (parts.Length < 2) { skipped++; continue; }

            try
            {
                var date = parts.Length > dateIdx ? parts[dateIdx].Trim() : "";
                var workoutName = parts.Length > nameIdx ? parts[nameIdx].Trim() : "Imported Workout";
                var exerciseName = parts.Length > exIdx ? parts[exIdx].Trim() : "";
                var notes = parts.Length > notesIdx ? parts[notesIdx].Trim() : "";

                if (string.IsNullOrWhiteSpace(exerciseName))
                {
                    skipped++;
                    continue;
                }

                var sets = 1;
                var reps = 0;
                var weightKg = 0.0;

                if (setsIdx >= 0 && parts.Length > setsIdx) int.TryParse(parts[setsIdx].Trim(), out sets);
                if (repsIdx >= 0 && parts.Length > repsIdx) int.TryParse(parts[repsIdx].Trim(), out reps);
                if (weightIdx >= 0 && parts.Length > weightIdx) double.TryParse(parts[weightIdx].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out weightKg);

                var isNewWorkout = currentDate != date || currentWorkoutName != workoutName;
                if (isNewWorkout && currentDate != null && currentExercises.Count > 0)
                {
                    await SaveWorkoutAsync(userId, userName, currentWorkoutName ?? "Workout", currentDate,
                        currentNotes, currentExercises, currentExerciseData);
                    imported++;
                    currentExercises.Clear();
                    currentExerciseData.Clear();
                }

                currentDate = date;
                currentWorkoutName = workoutName;
                currentNotes = notes;
                currentExercises.Add(exerciseName);
                currentExerciseData.Add(new
                {
                    name = exerciseName,
                    sets = new[] { new { setNumber = 1, weightKg, reps, isCompleted = true } }
                });
            }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[CsvImport] Row {i + 1} failed: {ex}");
                    errors.Add($"Riga {i + 1}: Errore durante l'importazione.");
            }
        }

        if (currentDate != null && currentExercises.Count > 0)
        {
            await SaveWorkoutAsync(userId, userName, currentWorkoutName ?? "Workout", currentDate,
                currentNotes, currentExercises, currentExerciseData);
            imported++;
        }

        return (imported, skipped, errors);
    }

    private async Task SaveWorkoutAsync(string userId, string userName, string name, string date,
        string? notes, List<string> exercises, List<object> exerciseData)
    {
        var volume = 0.0;
        var local = new LocalWorkout
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Name = name,
            Date = date,
            ExercisesJson = System.Text.Json.JsonSerializer.Serialize(exercises),
            Volume = volume,
            Duration = exercises.Count * 5,
            Notes = notes ?? "",
            ExerciseDataJson = System.Text.Json.JsonSerializer.Serialize(exerciseData),
            UserName = userName,
            PendingSync = true,
            LastSynced = DateTime.MinValue
        };

        await db.SaveWorkoutAsync(local);

        if (pb.IsLoggedIn)
        {
            try
            {
                var payload = new Dictionary<string, object?>
                {
                    ["user"] = userId,
                    ["user_name"] = userName,
                    ["name"] = name,
                    ["date"] = date,
                    ["notes"] = notes ?? "",
                    ["exercises"] = exercises,
                    ["exercise_data"] = System.Text.Json.JsonSerializer.Serialize(exerciseData),
                    ["volume"] = volume,
                    ["duration"] = exercises.Count * 5
                };
                var (ok, _) = await pb.CreateRecordAsync("logged_workouts", payload);
                if (ok)
                {
                    local.PendingSync = false;
                    local.LastSynced = DateTime.UtcNow;
                    await db.SaveWorkoutAsync(local);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CsvImport Save] ex: {ex.Message}");
            }
        }
    }

    private static int IndexOf(string[] header, params string[] names)
    {
        foreach (var name in names)
        {
            var idx = Array.FindIndex(header, h => h.Trim() == name);
            if (idx >= 0) return idx;
        }
        return -1;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = "";
        var inQuotes = false;

        foreach (var c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        result.Add(current);
        return result.ToArray();
    }
}
