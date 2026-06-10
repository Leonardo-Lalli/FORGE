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

    public async Task<List<ExerciseDbDto>> SearchExercisesAsync(string name, string? language = "2")
    {
        var lang = language ?? "2";
        var searchTerm = name.ToLowerInvariant();

        var cached = await db.GetCachedExercisesAsync();
        var matches = cached
            .Where(e => e.Name.ToLowerInvariant().Contains(searchTerm))
            .Take(15)
            .ToList();

        if (matches.Count >= 5)
            return MapToDto(matches);

        try
        {
            var url = $"exerciseinfo/?language={lang}&limit=100&offset=0";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse>(json, JsonOptions);

            if (result?.Results == null)
                return MapToDto(matches);

            var apiMatches = result.Results
                .Where(ex => ex.Translations?.FirstOrDefault()?.Name?.ToLowerInvariant().Contains(searchTerm) == true)
                .Take(15)
                .ToList();

            foreach (var ex in apiMatches)
            {
                var translation = ex.Translations?.FirstOrDefault();
                if (translation == null) continue;

                var cachedEx = new CachedExercise
                {
                    Id = $"wger-{ex.Id}",
                    Name = translation.Name ?? ex.Id.ToString(),
                    BodyPart = ex.Category?.Name ?? string.Empty,
                    Equipment = ex.Equipment?.FirstOrDefault()?.Name ?? string.Empty,
                    InstructionsJson = JsonSerializer.Serialize(new List<string> { translation.Description ?? "" }),
                    ImageUrl = ex.Images?.FirstOrDefault()?.Image ?? string.Empty,
                    Category = ex.Category?.Name ?? string.Empty,
                    Level = "",
                    Force = "",
                    Mechanic = ""
                };
                await db.SaveCachedExerciseAsync(cachedEx);

                if (!matches.Any(m => m.Id == cachedEx.Id))
                    matches.Add(cachedEx);
            }

            return MapToDto(matches);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger Search] ex: {ex.Message}");
            return MapToDto(matches);
        }
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
        var targetMuscle = muscle.ToLowerInvariant();

        var cached = await db.GetCachedExercisesAsync();
        var matches = cached
            .Where(e => e.BodyPart.ToLowerInvariant().Contains(targetMuscle)
                     || e.Category.ToLowerInvariant().Contains(targetMuscle))
            .Take(15)
            .ToList();

        if (matches.Count >= 5)
            return MapToDto(matches);

        try
        {
            var url = "exerciseinfo/?language=2&limit=100&offset=0";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse>(json, JsonOptions);

            if (result?.Results != null)
            {
                foreach (var ex in result.Results)
                {
                    var translation = ex.Translations?.FirstOrDefault();
                    if (translation == null) continue;

                    var bodyPart = (ex.Category?.Name ?? "").ToLowerInvariant();
                    var muscleNames = ex.Muscles?.Select(m => m.NameEn?.ToLowerInvariant() ?? "").ToList() ?? new();

                    if (!bodyPart.Contains(targetMuscle) && !muscleNames.Any(m => m.Contains(targetMuscle)))
                        continue;

                    var cachedEx = new CachedExercise
                    {
                        Id = $"wger-{ex.Id}",
                        Name = translation.Name ?? ex.Id.ToString(),
                        BodyPart = ex.Category?.Name ?? string.Empty,
                        Equipment = ex.Equipment?.FirstOrDefault()?.Name ?? string.Empty,
                        InstructionsJson = JsonSerializer.Serialize(new List<string> { translation.Description ?? "" }),
                        ImageUrl = ex.Images?.FirstOrDefault()?.Image ?? string.Empty,
                        Category = ex.Category?.Name ?? string.Empty
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    matches.Add(cachedEx);
                }
            }

            return MapToDto(matches.Take(15).ToList());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger Muscle] ex: {ex.Message}");
            return MapToDto(matches);
        }
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
