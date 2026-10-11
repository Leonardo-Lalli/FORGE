using System.Text.Json;
using Forge.Models;
using Forge.Models.Dto;

namespace Forge.Services;

public class ExerciseDbApiService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly DatabaseService db;
    private readonly PocketBaseService? pb;
    private HttpClient? _http;
    private bool warmDone;
    private int catalogTotal;
    private bool useServerCatalog;
    private readonly SemaphoreSlim warmGate = new(1, 1);

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
        await warmGate.WaitAsync();
        try
        {
            if (warmDone) return;
            var cacheKey = "exercise_catalog_paging_v3:" + Preferences.Get("pb_server_url", ServerAddress.PublicUrl);
            var statusJson = pb == null ? null : await pb.GetCatalogPageAsync(status: true);
            useServerCatalog = statusJson != null;
            var serverComplete = false;
            if (statusJson != null)
            {
                using var status = JsonDocument.Parse(statusJson);
                serverComplete = status.RootElement.GetProperty("complete").GetBoolean();
                catalogTotal = status.RootElement.GetProperty("total").GetInt32();
            }
            if (Preferences.Get(cacheKey, "") == "complete" && (!useServerCatalog || serverComplete)) { warmDone = true; return; }
            var pulled = await PullFromPocketBaseAsync();
            if (serverComplete && pulled >= catalogTotal)
            { warmDone = true; Preferences.Set(cacheKey, "complete"); return; }
            var fetched = await FetchFullCatalogAsync();
            if (catalogTotal > 0 && fetched >= catalogTotal) { warmDone = true; Preferences.Set(cacheKey, "complete"); }
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        { System.Diagnostics.Debug.WriteLine($"[Catalog] {ex.GetType().Name}"); }
        finally { warmGate.Release(); }
    }

    private async Task<int> FetchFullCatalogAsync()
    {
        var fetched = 0;
        var cursor = "";
        var consecutiveEmpty = 0;
        var ids = new HashSet<string>();
        var cursors = new HashSet<string>();

        while (consecutiveEmpty < 5)
        {
            var url = $"exercises?limit=25{(string.IsNullOrEmpty(cursor) ? "" : $"&after={Uri.EscapeDataString(cursor)}")}";

            var json = useServerCatalog && pb != null ? await pb.GetCatalogPageAsync(cursor) : null;
            if (json == null)
            {
                HttpResponseMessage? response = null;
                for (var retry = 0; retry < 3; retry++)
                {
                    try { response = await GetHttp().GetAsync(url); if (response.IsSuccessStatusCode) break; }
                    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { }
                    await Task.Delay(500 * (retry + 1));
                }
                if (response == null || !response.IsSuccessStatusCode)
                {
                    response?.Dispose(); consecutiveEmpty++; await Task.Delay(2000); continue;
                }
                using (response) { json = await response.Content.ReadAsStringAsync(); }
            }
            var result = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json, JsonOptions);
            if (result?.Data == null || result.Data.Count == 0) { consecutiveEmpty++; continue; }
            consecutiveEmpty = 0;
            catalogTotal = result.Meta?.Total ?? 0;

            foreach (var ex in result.Data)
            {
                if (string.IsNullOrWhiteSpace(ex.ExerciseId)) continue;
                await db.SaveCachedExerciseAsync(new CachedExercise
                {
                    Id = ex.ExerciseId,
                    SourceExerciseId = ex.ExerciseId,
                    Name = Capitalize(ex.Name),
                    BodyPart = string.Join(", ", ex.GetBodyParts()),
                    Equipment = string.Join(", ", ex.GetEquipments()),
                    InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                    ImageUrl = ex.GifUrl,
                    Category = ex.GetBodyParts().FirstOrDefault() ?? "",
                    TargetMuscles = string.Join(", ", ex.GetTargetMuscles()),
                    SecondaryMuscles = string.Join(", ", ex.GetSecondaryMuscles())
                });
                ids.Add(ex.ExerciseId);
                fetched = ids.Count;
            }

            if (result.Meta?.HasNextPage == true)
            {
                cursor = result.Meta.NextCursor;
                if (string.IsNullOrEmpty(cursor) || !cursors.Add(cursor)) break;
            }
            else break;

            await Task.Delay(1200);
        }
        return fetched;
    }

    private async Task<int> PullFromPocketBaseAsync()
    {
        if (pb == null || !pb.IsLoggedIn) return 0;
        var pulled = 0;
        try
        {
            var page = 1;
            var seen = new HashSet<string>();
            while (true)
            {
                var records = await pb.FetchExcercisePageAsync(page, 100);
                if (records == null || records.Count == 0) break;
                if (!records.Any(r => seen.Add(r.Id ?? ""))) break;

                foreach (var r in records)
                {
                    var cachedEx = new CachedExercise
                    {
                        Id = string.IsNullOrWhiteSpace(r.ExerciseId) ? r.Id ?? "" : r.ExerciseId,
                        SourceExerciseId = r.ExerciseId ?? "",
                        Name = r.Name ?? "",
                        BodyPart = r.BodyPart ?? "",
                        Equipment = r.Equipment ?? "",
                        InstructionsJson = JsonSerializer.Serialize(r.Instructions ?? new List<string>()),
                        ImageUrl = r.ImageUrl ?? "",
                        Category = r.Category ?? "",
                        TargetMuscles = r.TargetMuscles ?? "",
                        SecondaryMuscles = r.SecondaryMuscles ?? ""
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    if (!string.IsNullOrWhiteSpace(r.ExerciseId) && !string.IsNullOrWhiteSpace(r.ImageUrl) && r.Instructions is { Count: > 0 }) pulled++;
                }

                if (records.Count < 100) break;
                page++;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB PullFromPB] ex: {ex.Message}");
        }
        return pulled;
    }

    public async Task<List<ExerciseSearchResultDto>> SearchAsync(string name)
    {
        await WarmCacheAsync();
        var searchTerm = TranslateSearchTerm(name.ToLowerInvariant());
        var originalTerm = name.ToLowerInvariant();

        var cached = await GetCatalogAsync();
        var matches = cached
            .Where(e => e.Name.ToLowerInvariant().Contains(searchTerm)
                     || e.Name.ToLowerInvariant().Contains(originalTerm)
                     || e.BodyPart.ToLowerInvariant().Contains(originalTerm)
                     || e.BodyPart.ToLowerInvariant().Contains(searchTerm)
                     || e.Equipment.ToLowerInvariant().Contains(originalTerm)
                     || e.Equipment.ToLowerInvariant().Contains(searchTerm)
                     || e.TargetMuscles.ToLowerInvariant().Contains(searchTerm)
                     || e.SecondaryMuscles.ToLowerInvariant().Contains(searchTerm))
            .ToList();

        return matches.Select(ToSearchResult).ToList();
    }

    public async Task<List<CachedExercise>> GetCatalogAsync() => (await db.GetCachedExercisesAsync())
        .GroupBy(e => e.Name.Trim(), StringComparer.OrdinalIgnoreCase)
        .SelectMany(group => group.Any(e => !string.IsNullOrWhiteSpace(e.SourceExerciseId))
            ? group.Where(e => !string.IsNullOrWhiteSpace(e.SourceExerciseId))
            : group.OrderByDescending(e =>
            (!string.IsNullOrWhiteSpace(e.ImageUrl) ? 4 : 0) +
            (!string.IsNullOrWhiteSpace(e.InstructionsJson) && e.InstructionsJson != "[]" ? 4 : 0) +
            (!string.IsNullOrWhiteSpace(e.TargetMuscles) ? 1 : 0)).Take(1))
        .OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase).ToList();

    public async Task<List<ExerciseSearchResultDto>> GetByBodyPartAsync(string bodyPart)
    {
        var target = bodyPart.ToLowerInvariant();
        var cached = await db.GetCachedExercisesAsync();
        return cached
            .Where(e => e.BodyPart.ToLowerInvariant().Contains(target)
                     || e.TargetMuscles.ToLowerInvariant().Contains(target)
                     || e.SecondaryMuscles.ToLowerInvariant().Contains(target)
                     || e.Category.ToLowerInvariant().Contains(target))
            .Select(ToSearchResult)
            .ToList();
    }

    public async Task<List<ExerciseSearchResultDto>> GetByEquipmentAsync(string equipment)
    {
        var target = equipment.ToLowerInvariant();
        var cached = await db.GetCachedExercisesAsync();
        return cached
            .Where(e => e.Equipment.ToLowerInvariant().Contains(target))
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

    private static ExerciseSearchResultDto ToSearchResult(CachedExercise e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        BodyPart = e.BodyPart,
        Equipment = e.Equipment,
        TargetMuscles = e.TargetMuscles,
        SecondaryMuscles = e.SecondaryMuscles,
        Category = e.Category,
        ImageUrl = e.ImageUrl,
        Instructions = JsonSerializer.Deserialize<List<string>>(e.InstructionsJson) ?? new()
    };

    private static string Capitalize(string name) =>
        string.Join(" ", name.Split(' ').Where(w => w.Length > 0).Select(w =>
            w.Length > 2 ? char.ToUpper(w[0]) + w[1..] : w.ToUpperInvariant()));

    private static readonly Dictionary<string, string> ItalianToEnglish = new(StringComparer.OrdinalIgnoreCase)
    {
        ["panca"] = "bench", ["panche"] = "bench",
        ["squat"] = "squat", ["stacco"] = "deadlift",
        ["trazioni"] = "pull up", ["trazione"] = "pull up",
        ["flessioni"] = "push up", ["flessione"] = "push up",
        ["curl"] = "curl", ["spinte"] = "press",
        ["spinta"] = "press", ["rematore"] = "row",
        ["alzate"] = "raise", ["alzata"] = "raise",
        ["addominali"] = "abs", ["addome"] = "abs",
        ["gambe"] = "leg", ["gamba"] = "leg",
        ["braccia"] = "arm", ["braccio"] = "arm",
        ["petto"] = "chest", ["pettorali"] = "pectorals",
        ["schiena"] = "back", ["dorsali"] = "lats", ["dorso"] = "back",
        ["spalle"] = "shoulder", ["spalla"] = "shoulder",
        ["bicipiti"] = "biceps", ["bicipite"] = "biceps",
        ["tricipiti"] = "triceps", ["tricipite"] = "triceps",
        ["quadricipiti"] = "quadriceps", ["quadricipite"] = "quadriceps",
        ["femorali"] = "hamstring", ["polpacci"] = "calves",
        ["glutei"] = "glutes", ["gluteo"] = "glutes",
        ["manubri"] = "dumbbell", ["manubrio"] = "dumbbell",
        ["bilanciere"] = "barbell", ["elastico"] = "band",
        ["cavi"] = "cable", ["macchinari"] = "machine",
        ["macchinario"] = "machine", ["corpo libero"] = "body weight",
        ["kettlebell"] = "kettlebell", ["affondi"] = "lunge",
        ["affondo"] = "lunge", ["ponte"] = "bridge",
        ["plank"] = "plank", ["crunch"] = "crunch",
        ["tirate"] = "pull", ["tirata"] = "pull",
    };

    private static string TranslateSearchTerm(string term)
    {
        var words = term.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (ItalianToEnglish.TryGetValue(words[i], out var translated))
                words[i] = translated;
        }
        return string.Join(" ", words);
    }
}

public class ExerciseSearchResultDto
{
    public string TargetMuscles { get; set; } = "";
    public string SecondaryMuscles { get; set; } = "";
    public string Category { get; set; } = "";
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string BodyPart { get; set; } = "";
    public string Equipment { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public List<string> Instructions { get; set; } = new();
}
