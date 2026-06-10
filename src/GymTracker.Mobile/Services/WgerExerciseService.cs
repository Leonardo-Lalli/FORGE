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
        var searchTerm = exerciseName.ToLowerInvariant();

        var cached = await db.GetCachedExercisesAsync();
        var match = cached.FirstOrDefault(e => e.Name.ToLowerInvariant().Contains(searchTerm)
                                            || searchTerm.Contains(e.Name.ToLowerInvariant()));
        if (match != null && !string.IsNullOrWhiteSpace(match.ImageUrl))
            return match.ImageUrl;

        try
        {
            var url = "exerciseinfo/?language=2&limit=200&offset=0";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse>(json, JsonOptions);

            if (result?.Results != null)
            {
                foreach (var ex in result.Results)
                {
                    var translation = ex.Translations?.FirstOrDefault();
                    var name = translation?.Name?.ToLowerInvariant() ?? "";

                    if (!name.Contains(searchTerm) && !searchTerm.Contains(name))
                        continue;

                    var img = ex.Images?.FirstOrDefault()?.Image;
                    if (string.IsNullOrWhiteSpace(img)) continue;

                    var cachedEx = new CachedExercise
                    {
                        Id = $"wger-{ex.Id}",
                        Name = translation?.Name ?? ex.Id.ToString(),
                        BodyPart = ex.Category?.Name ?? "",
                        ImageUrl = img
                    };
                    await db.SaveCachedExerciseAsync(cachedEx);
                    return img;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger GetImg] ex: {ex.Message}");
        }

        return null;
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
