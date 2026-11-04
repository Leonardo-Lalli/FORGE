using System.Text;

namespace Forge.Services;

public class CsvExportService
{
    private readonly DatabaseService db;
    private readonly WeightUnitService units;

    public CsvExportService(DatabaseService db, WeightUnitService units)
    {
        this.db = db;
        this.units = units;
    }

    public async Task<string> ExportWorkoutsAsync(string userId)
        => CsvWorkoutWriter.Write(await db.GetWorkoutsAsync(userId), units);

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
