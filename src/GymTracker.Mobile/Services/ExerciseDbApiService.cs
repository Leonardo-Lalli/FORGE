using System.Text.Json;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class ExerciseDbApiService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly DatabaseService db;
    private readonly PocketBaseService? pb;
    private HttpClient? _http;
    private bool warmDone;

    private HttpClient GetHttp()
    {
        if (_http != null) return _http;
        _http = httpFactory.CreateClient("exercisedbv1");
        _http.BaseAddress = new Uri("https://oss.exercisedb.dev/api/v1/");
        _http.Timeout = TimeSpan.FromSeconds(15);
        return _http;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ExerciseDbApiService(IHttpClientFactory httpFactory, DatabaseService db, PocketBaseService pb)
    {
        this.httpFactory = httpFactory;
        this.db = db;
        this.pb = pb;
    }

    public async Task WarmCacheAsync()
    {
        if (warmDone) return;
        warmDone = true;

        await db.DeleteExercisesByPrefixAsync("wger-");
        var count = (await db.GetCachedExercisesAsync()).Count;
        if (count >= 100) return;

        try
        {
            var cursor = "";
            var fetched = 0;
            while (true)
            {
                var url = $"exercises?limit=100{(string.IsNullOrEmpty(cursor) ? "" : $"&cursor={Uri.EscapeDataString(cursor)}")}";
                var response = await GetHttp().GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json, JsonOptions);
                if (result?.Data == null || result.Data.Count == 0) break;

                foreach (var ex in result.Data)
                {
                    if (string.IsNullOrWhiteSpace(ex.GifUrl)) continue;
                    var cachedEx = new CachedExercise
                    {
                        Id = ex.ExerciseId,
                        Name = Capitalize(ex.Name),
                        BodyPart = string.Join(", ", ex.GetBodyParts()),
                        Equipment = string.Join(", ", ex.GetEquipments()),
                        InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                        ImageUrl = ex.GifUrl,
                        Category = ex.GetBodyParts().FirstOrDefault() ?? ""
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    fetched++;
                }

                if (!result.Meta?.HasNextPage == true)
                    cursor = result.Meta!.NextCursor;
                else
                    break;
            }
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] cached {fetched} exercises");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] ex: {ex.Message}");
        }

        _ = SyncToPocketBaseAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                System.Diagnostics.Debug.WriteLine($"[ExerciseDB Sync] ex: {t.Exception?.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    public async Task<List<ExerciseSearchResultDto>> SearchAsync(string name)
    {
        var searchTerm = name.ToLowerInvariant();

        var cached = await db.GetCachedExercisesAsync();
        var matches = cached
            .Where(e => e.Name.ToLowerInvariant().Contains(searchTerm))
            .Take(20)
            .ToList();

        if (matches.Count >= 5)
            return matches.Select(ToSearchResult).ToList();

        try
        {
            var url = $"exercises?name={Uri.EscapeDataString(name)}&limit=20";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json, JsonOptions);

            if (result?.Data != null)
            {
                foreach (var ex in result.Data)
                {
                    var cachedEx = new CachedExercise
                    {
                        Id = ex.ExerciseId,
                        Name = Capitalize(ex.Name),
                        BodyPart = ex.GetBodyParts().FirstOrDefault() ?? "",
                        Equipment = ex.GetEquipments().FirstOrDefault() ?? "",
                        InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                        ImageUrl = ex.GifUrl,
                        Category = ex.GetBodyParts().FirstOrDefault() ?? ""
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    if (!matches.Any(m => m.Id == ex.ExerciseId))
                        matches.Add(cachedEx);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB Search] ex: {ex.Message}");
        }

        return matches.Take(20).Select(ToSearchResult).ToList();
    }

    public async Task<List<ExerciseSearchResultDto>> GetByBodyPartAsync(string bodyPart)
    {
        var target = bodyPart.ToLowerInvariant();
        var cached = await db.GetCachedExercisesAsync();
        return cached
            .Where(e => e.BodyPart.ToLowerInvariant().Contains(target)
                     || e.Category.ToLowerInvariant().Contains(target))
            .Take(20)
            .Select(ToSearchResult)
            .ToList();
    }

    public async Task<List<ExerciseSearchResultDto>> GetByEquipmentAsync(string equipment)
    {
        var target = equipment.ToLowerInvariant();
        var cached = await db.GetCachedExercisesAsync();
        return cached
            .Where(e => e.Equipment.ToLowerInvariant().Contains(target))
            .Take(20)
            .Select(ToSearchResult)
            .ToList();
    }

    public async Task<ExerciseDbV1Dto?> GetByIdAsync(string exerciseId)
    {
        try
        {
            var url = $"exercises/{Uri.EscapeDataString(exerciseId)}";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExerciseDbV1DetailResponse>(json, JsonOptions);
            return result?.Data;
        }
        catch
        {
            return null;
        }
    }

    private async Task SyncToPocketBaseAsync()
    {
        if (pb == null || !pb.IsLoggedIn) return;
        try
        {
            var cached = await db.GetCachedExercisesAsync();
            var synced = 0;
            foreach (var ex in cached.Take(500))
            {
                try
                {
                    var instructions = JsonSerializer.Deserialize<List<string>>(ex.InstructionsJson) ?? new();
                    var payload = new Dictionary<string, object?>
                    {
                        ["name"] = ex.Name,
                        ["bodyPart"] = ex.BodyPart,
                        ["equipment"] = ex.Equipment,
                        ["instructions"] = instructions,
                        ["imageUrl"] = ex.ImageUrl,
                        ["category"] = ex.Category
                    };
                    await pb.CreateRecordAsync("excercise", payload);
                    synced++;
                }
                catch (Exception exc) {
                    System.Diagnostics.Debug.WriteLine($"[ExerciseDB Sync] skip: {exc.Message}");
                }
            }
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB Sync] synced {synced} to PocketBase");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB Sync] ex: {ex.Message}");
        }
    }

    private static ExerciseSearchResultDto ToSearchResult(CachedExercise e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        BodyPart = e.BodyPart,
        Equipment = e.Equipment,
        ImageUrl = e.ImageUrl,
        Instructions = JsonSerializer.Deserialize<List<string>>(e.InstructionsJson) ?? new()
    };

    private static string Capitalize(string name) =>
        string.Join(" ", name.Split(' ').Where(w => w.Length > 0).Select(w =>
            w.Length > 2 ? char.ToUpper(w[0]) + w[1..] : w.ToUpperInvariant()));
}

public class ExerciseSearchResultDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string BodyPart { get; set; } = "";
    public string Equipment { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public List<string> Instructions { get; set; } = new();
}
