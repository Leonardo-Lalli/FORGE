namespace GymTracker.Mobile.Services;

public class BuildSecrets
{
    private readonly Dictionary<string, string> secrets = new(StringComparer.OrdinalIgnoreCase);

    public BuildSecrets()
    {
    }

    public async Task LoadAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("gymtracker.env");
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } line)
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    continue;

                var eq = line.IndexOf('=');
                if (eq < 1)
                    continue;

                var key = line[..eq].Trim();
                var value = line[(eq + 1)..].Trim();
                secrets[key] = value;
            }
        }
        catch (FileNotFoundException)
        {
            System.Diagnostics.Debug.WriteLine("[BuildSecrets] gymtracker.env not found — running without API keys");
        }
    }

    public string? Get(string key)
    {
        return secrets.TryGetValue(key, out var value) ? value : null;
    }
}
