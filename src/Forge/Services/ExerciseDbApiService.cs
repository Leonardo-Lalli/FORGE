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
        await db.DeleteExercisesByPrefixAsync("wger-");
        var count = (await db.GetCachedExercisesAsync()).Count;

        // Force full rebuild if cache is from old code (< 200 exercises = old limit was 100)
        if (count is > 0 and < 200)
        {
            System.Diagnostics.Debug.WriteLine("[ExerciseDB WarmCache] Old cache detected, full rebuild...");
            await db.DeleteAllExercisesAsync();
            count = 0;
        }

        if (count >= 600) { warmDone = true; return; }

        try
        {
            // 1. Try pulling from PocketBase first (shared server cache, instant)
            var fromPb = await PullFromPocketBaseAsync();
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] pulled {fromPb} from PocketBase");

            count = (await db.GetCachedExercisesAsync()).Count;
            if (count < 100)
            {
                // 2. First launch: aggressive fetch from API (happens once, ever)
                var fromApi = await FetchFullCatalogAsync();
                System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] initial fetch: {fromApi} from API");
            }
            else if (count < 600)
            {
                // 3. Partial cache: fetch a chunk to fill up
                var fromApi = await FetchExerciseDbChunkAsync(100);
                System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] chunk fetch: {fromApi} from API");
            }

            count = (await db.GetCachedExercisesAsync()).Count;
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] total: {count}");

            // 4. Sync to PocketBase so other users on the same server get them
            _ = SyncToPocketBaseAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    System.Diagnostics.Debug.WriteLine($"[ExerciseDB Sync] ex: {t.Exception?.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseDB WarmCache] ex: {ex.Message}");
        }
    }

    private async Task<int> FetchFullCatalogAsync()
    {
        var fetched = 0;
        var cursor = "";
        var consecutiveEmpty = 0;

        while (fetched < 1500 && consecutiveEmpty < 5)
        {
            var url = $"exercises?limit=25{(string.IsNullOrEmpty(cursor) ? "" : $"&cursor={Uri.EscapeDataString(cursor)}")}";

            HttpResponseMessage? response = null;
            for (var retry = 0; retry < 3; retry++)
            {
                try { response = await GetHttp().GetAsync(url); if (response.IsSuccessStatusCode) break; }
                catch { }
                await Task.Delay(500 * (retry + 1));
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                consecutiveEmpty++;
                await Task.Delay(2000);
                continue;
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json, JsonOptions);
            if (result?.Data == null || result.Data.Count == 0) { consecutiveEmpty++; continue; }
            consecutiveEmpty = 0;

            foreach (var ex in result.Data)
            {
                if (string.IsNullOrWhiteSpace(ex.GifUrl)) continue;
                await db.SaveCachedExerciseAsync(new CachedExercise
                {
                    Id = ex.ExerciseId,
                    Name = Capitalize(ex.Name),
                    BodyPart = string.Join(", ", ex.GetBodyParts()),
                    Equipment = string.Join(", ", ex.GetEquipments()),
                    InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                    ImageUrl = ex.GifUrl,
                    Category = ex.GetBodyParts().FirstOrDefault() ?? "",
                    TargetMuscles = string.Join(", ", ex.GetTargetMuscles()),
                    SecondaryMuscles = string.Join(", ", ex.GetSecondaryMuscles())
                });
                fetched++;
            }

            if (result.Meta?.HasNextPage == true)
                cursor = result.Meta.NextCursor;
            else break;

            await Task.Delay(400);
        }
        return fetched;
    }

    private async Task<int> FetchExerciseDbChunkAsync(int targetCount)
    {
        var fetched = 0;
        var cursor = Preferences.Get("exercise_cache_cursor", "");

        while (fetched < targetCount)
        {
            var url = $"exercises?limit=25{(string.IsNullOrEmpty(cursor) ? "" : $"&cursor={Uri.EscapeDataString(cursor)}")}";

            HttpResponseMessage? response = null;
            for (var retry = 0; retry < 3; retry++)
            {
                try { response = await GetHttp().GetAsync(url); if (response.IsSuccessStatusCode) break; }
                catch { }
                await Task.Delay(500 * (retry + 1));
            }

            if (response == null || !response.IsSuccessStatusCode) break;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExerciseDbV1ListResponse>(json, JsonOptions);
            if (result?.Data == null || result.Data.Count == 0) break;

            foreach (var ex in result.Data)
            {
                if (string.IsNullOrWhiteSpace(ex.GifUrl)) continue;
                await db.SaveCachedExerciseAsync(new CachedExercise
                {
                    Id = ex.ExerciseId,
                    Name = Capitalize(ex.Name),
                    BodyPart = string.Join(", ", ex.GetBodyParts()),
                    Equipment = string.Join(", ", ex.GetEquipments()),
                    InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                    ImageUrl = ex.GifUrl,
                    Category = ex.GetBodyParts().FirstOrDefault() ?? "",
                    TargetMuscles = string.Join(", ", ex.GetTargetMuscles()),
                    SecondaryMuscles = string.Join(", ", ex.GetSecondaryMuscles())
                });
                fetched++;
            }

            if (result.Meta?.HasNextPage == true)
            {
                cursor = result.Meta.NextCursor;
                Preferences.Set("exercise_cache_cursor", cursor);
            }
            else break;

            await Task.Delay(400);
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
            while (page <= 10)
            {
                var records = await pb.FetchExcercisePageAsync(page, 100);
                if (records == null || records.Count == 0) break;

                foreach (var r in records)
                {
                    var existing = await db.GetCachedExerciseByIdAsync(r.Id ?? "");
                    if (existing != null) continue;

                    var cachedEx = new CachedExercise
                    {
                        Id = r.Id ?? "",
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
                    pulled++;
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
        var searchTerm = TranslateSearchTerm(name.ToLowerInvariant());
        var originalTerm = name.ToLowerInvariant();

        var cached = await db.GetCachedExercisesAsync();
        var matches = cached
            .Where(e => e.Name.ToLowerInvariant().Contains(searchTerm)
                     || e.Name.ToLowerInvariant().Contains(originalTerm)
                     || e.BodyPart.ToLowerInvariant().Contains(originalTerm)
                     || e.Equipment.ToLowerInvariant().Contains(originalTerm))
            .ToList();

        if (matches.Count >= 5)
            return matches.Select(ToSearchResult).ToList();

        try
        {
            var url = $"exercises?name={Uri.EscapeDataString(searchTerm)}&limit=20";
            var response = await GetHttp().GetAsync(url);
            if (!response.IsSuccessStatusCode) return matches.Select(ToSearchResult).ToList();

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
                        BodyPart = string.Join(", ", ex.GetBodyParts()),
                        Equipment = string.Join(", ", ex.GetEquipments()),
                        InstructionsJson = JsonSerializer.Serialize(ex.GetInstructions()),
                        ImageUrl = ex.GifUrl,
                        Category = ex.GetBodyParts().FirstOrDefault() ?? "",
                        TargetMuscles = string.Join(", ", ex.GetTargetMuscles()),
                        SecondaryMuscles = string.Join(", ", ex.GetSecondaryMuscles())
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

        return matches.Select(ToSearchResult).ToList();
    }

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

    private async Task SyncToPocketBaseAsync()
    {
        if (pb == null || !pb.IsLoggedIn) return;
        try
        {
            var cached = await db.GetCachedExercisesAsync();
            var synced = 0;
            foreach (var ex in cached.Take(750))
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
                        ["category"] = ex.Category,
                        ["targetMuscles"] = ex.TargetMuscles,
                        ["secondaryMuscles"] = ex.SecondaryMuscles
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
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string BodyPart { get; set; } = "";
    public string Equipment { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public List<string> Instructions { get; set; } = new();
}
