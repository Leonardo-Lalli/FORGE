using System.Collections.Concurrent;

namespace GymTracker.Mobile.Services;

public class BuildSecrets
{
    private readonly ConcurrentDictionary<string, string> secrets = new(StringComparer.OrdinalIgnoreCase);

    public async Task LoadAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("forge.env");
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BuildSecrets] Failed to load forge.env: {ex.Message}");
        }
    }

    public string? Get(string key)
    {
        return secrets.TryGetValue(key, out var value) ? value : null;
    }
}
