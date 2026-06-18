using System.Text;

namespace Forge.Services;

public class CsvExportService
{
    private readonly DatabaseService db;

    public CsvExportService(DatabaseService db)
    {
        this.db = db;
    }

    public async Task<string> ExportWorkoutsAsync(string userId)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,WorkoutName,ExerciseName,Sets,Reps,WeightKg,Notes");

        var workouts = await db.GetWorkoutsAsync(userId);

        foreach (var w in workouts.OrderBy(w => w.Date))
        {
            var exercises = System.Text.Json.JsonSerializer.Deserialize<List<string>>(w.ExercisesJson) ?? new();

            foreach (var ex in exercises)
            {
                sb.AppendLine($"\"{w.Date}\",\"{w.Name}\",\"{ex}\",1,0,0,\"{w.Notes}\"");
            }
        }

        return sb.ToString();
    }

    public async Task SaveCsvFileAsync(string userId)
    {
        var csv = await ExportWorkoutsAsync(userId);
        var fileName = $"FORGE_export_{DateTime.Now:yyyy-MM-dd}.csv";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, csv);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Esporta allenamenti FORGE",
            File = new ShareFile(filePath)
        });
    }
}
