using System.Text.Json;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class WgerExerciseService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly DatabaseService db;
    private HttpClient? _http;

    private HttpClient GetHttp()
    {
        if (_http != null) return _http;
        _http = httpFactory.CreateClient("wger");
        _http.BaseAddress = new Uri("https://wger.de/api/v2/");
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("FORGE/1.0");
        _http.Timeout = TimeSpan.FromSeconds(15);
        return _http;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WgerExerciseService(IHttpClientFactory httpFactory, DatabaseService db)
    {
        this.httpFactory = httpFactory;
        this.db = db;
    }

    public async Task WarmCacheAsync()
    {
        var count = (await db.GetCachedExercisesAsync()).Count;
        if (count >= 100) return;

        try
        {
            for (int offset = 0; offset < 800; offset += 200)
            {
                var url = $"exerciseinfo/?language=2&limit=200&offset={offset}";
                var response = await GetHttp().GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WgerListResponse>(json, JsonOptions);
                if (result?.Results == null || result.Results.Count == 0) break;

                foreach (var ex in result.Results)
                {
                    var translation = ex.Translations?.FirstOrDefault();
                    if (translation == null) continue;
                    var img = ex.Images?.FirstOrDefault()?.Image;
                    if (string.IsNullOrWhiteSpace(img)) continue;

                    var cachedEx = new CachedExercise
                    {
                        Id = $"wger-{ex.Id}",
                        Name = translation.Name ?? "",
                        BodyPart = ex.Category?.Name ?? "",
                        Equipment = ex.Equipment?.FirstOrDefault()?.Name ?? "",
                        InstructionsJson = JsonSerializer.Serialize(new List<string> { translation.Description ?? "" }),
                        ImageUrl = img,
                        Category = ex.Category?.Name ?? ""
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                }
            }
            var afterCount = (await db.GetCachedExercisesAsync()).Count;
            System.Diagnostics.Debug.WriteLine($"[Wger WarmCache] cached {afterCount} exercises (+{afterCount - count})");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger WarmCache] ex: {ex.Message}");
        }
    }

    public async Task<List<ExerciseDbDto>> SearchLocalAsync(string name)
    {
        var searchWords = name.ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();
        if (searchWords.Count == 0) return new();

        var cached = await db.GetCachedExercisesAsync();
        var scored = new List<(CachedExercise Ex, int Score)>();

        foreach (var ex in cached)
        {
            var score = CountMatchingWords(ex.Name, searchWords);
            score += CountMatchingWords(ex.BodyPart, searchWords);
            score += CountMatchingWords(ex.Equipment, searchWords);
            if (score > 0) scored.Add((ex, score));
        }

        return scored
            .OrderByDescending(s => s.Score)
            .Take(15)
            .Select(s => new ExerciseDbDto
            {
                Id = s.Ex.Id,
                Name = s.Ex.Name,
                PrimaryMuscles = new List<string> { s.Ex.BodyPart },
                Equipment = s.Ex.Equipment,
                Instructions = JsonSerializer.Deserialize<List<string>>(s.Ex.InstructionsJson) ?? new(),
                Images = string.IsNullOrWhiteSpace(s.Ex.ImageUrl) ? new() : new List<string> { s.Ex.ImageUrl },
                Category = s.Ex.Category
            }).ToList();
    }

    public async Task<string?> GetImageForExerciseAsync(string exerciseName)
    {
        var searchWords = exerciseName.ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.TrimEnd(',', '.', '-'))
            .Where(w => w.Length > 2)
            .ToHashSet();
        if (searchWords.Count == 0) return null;

        var cached = await db.GetCachedExercisesAsync();
        var match = FindBestMatch(cached.Select(e => e.Name), searchWords);
        if (match != null)
        {
            var cachedMatch = cached.First(e => e.Name == match);
            if (!string.IsNullOrWhiteSpace(cachedMatch.ImageUrl))
                return cachedMatch.ImageUrl;
        }

        try
        {
            for (int offset = 0; offset < 600; offset += 200)
            {
                var url = $"exerciseinfo/?language=2&limit=200&offset={offset}";
                var response = await GetHttp().GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WgerListResponse>(json, JsonOptions);
                if (result?.Results == null || result.Results.Count == 0) break;

                var bestEx = (WgerExerciseInfo?)null;
                var bestScore = 0;
                var bestName = "";

                foreach (var ex in result.Results)
                {
                    var translation = ex.Translations?.FirstOrDefault();
                    var name = translation?.Name ?? "";
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var score = CountMatchingWords(name, searchWords);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestEx = ex;
                        bestName = name;
                    }
                }

                if (bestEx != null && bestScore >= 2)
                {
                    var img = bestEx.Images?.FirstOrDefault()?.Image;
                    var cachedEx = new CachedExercise
                    {
                        Id = $"wger-{bestEx.Id}",
                        Name = bestName,
                        BodyPart = bestEx.Category?.Name ?? "",
                        ImageUrl = img ?? ""
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    if (!string.IsNullOrWhiteSpace(img)) return img;
                }

                if (result.Results.Count < 200) break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger GetImg] ex: {ex.Message}");
        }

        return null;
    }

    private static string? FindBestMatch(IEnumerable<string> names, HashSet<string> searchWords)
    {
        var bestScore = 0;
        string? bestName = null;
        foreach (var name in names)
        {
            var score = CountMatchingWords(name, searchWords);
            if (score > bestScore) { bestScore = score; bestName = name; }
        }
        return bestScore >= 2 ? bestName : null;
    }

    private static int CountMatchingWords(string name, HashSet<string> searchWords)
    {
        var nameWords = name.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.TrimEnd(',', '.', '-')).ToHashSet();
        return searchWords.Count(w => nameWords.Contains(w));
    }

    public async Task<List<ExerciseDbDto>> GetByMuscleAsync(string muscle)
    {
        var target = muscle.ToLowerInvariant();
        var cached = await db.GetCachedExercisesAsync();
        var matches = cached
            .Where(e => e.BodyPart.ToLowerInvariant().Contains(target)
                     || e.Category.ToLowerInvariant().Contains(target))
            .Take(15)
            .ToList();

        return MapToDto(matches);
    }

    private static List<ExerciseDbDto> MapToDto(List<CachedExercise> exercises)
    {
        return exercises.Select(e => new ExerciseDbDto
        {
            Id = e.Id,
            Name = e.Name,
            PrimaryMuscles = new List<string> { e.BodyPart },
            Equipment = e.Equipment,
            Instructions = JsonSerializer.Deserialize<List<string>>(e.InstructionsJson) ?? new(),
            Images = string.IsNullOrWhiteSpace(e.ImageUrl) ? new() : new List<string> { e.ImageUrl },
            Category = e.Category
        }).ToList();
    }

    private class WgerListResponse
    {
        public int Count { get; set; }
        public List<WgerExerciseInfo> Results { get; set; } = new();
    }

    private class WgerExerciseInfo
    {
        public int Id { get; set; }
        public WgerCategory? Category { get; set; }
        public List<WgerMuscle>? Muscles { get; set; }
        public List<WgerEquipment>? Equipment { get; set; }
        public List<WgerImage>? Images { get; set; }
        public List<WgerTranslation>? Translations { get; set; }
    }

    private class WgerCategory
    {
        public string? Name { get; set; }
    }

    private class WgerMuscle
    {
        public string? Name { get; set; }
        public string? NameEn { get; set; }
    }

    private class WgerEquipment
    {
        public string? Name { get; set; }
    }

    private class WgerImage
    {
        public string? Image { get; set; }
        public bool IsMain { get; set; }
    }

    private class WgerTranslation
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
