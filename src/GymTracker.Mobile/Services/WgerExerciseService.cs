using System.Text.Json;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class WgerExerciseService
{
    private readonly IHttpClientFactory httpFactory;
    private HttpClient? _http;

    private HttpClient GetHttp()
    {
        if (_http != null) return _http;
        _http = httpFactory.CreateClient("wger");
        _http.BaseAddress = new Uri("https://wger.de/api/v2/");
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("FORGE/1.0");
        _http.Timeout = TimeSpan.FromSeconds(10);
        return _http;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WgerExerciseService(IHttpClientFactory httpFactory)
    {
        this.httpFactory = httpFactory;
    }

    public async Task<List<ExerciseDbDto>> SearchExercisesAsync(string name, string? language = "2")
    {
        try
        {
            var lang = language ?? "2";
            var url = $"exercise/search/?language={lang}&limit=15&term={Uri.EscapeDataString(name)}";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse<WgerExercise>>(json, JsonOptions);

            if (result?.Results == null || result.Results.Count == 0)
                return new();

            var exercises = new List<ExerciseDbDto>();
            foreach (var ex in result.Results)
            {
                var dto = new ExerciseDbDto
                {
                    Id = $"wger-{ex.Id}",
                    Name = ex.Name,
                    Category = ex.Category?.Name ?? "",
                    Equipment = ex.Equipment?.Select(e => e.Name).FirstOrDefault() ?? "",
                    PrimaryMuscles = ex.Muscles?.Select(m => m.Name).Where(n => n != null).Cast<string>().ToList() ?? new(),
                    Instructions = ex.Description != null ? new List<string> { ex.Description } : new(),
                    Images = new List<string>()
                };

                var imageUrl = await GetExerciseImageUrlAsync(ex.Id);
                if (!string.IsNullOrWhiteSpace(imageUrl))
                    dto.Images.Add(imageUrl);

                exercises.Add(dto);
            }

            return exercises;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger Search] ex: {ex.Message}");
            return new();
        }
    }

    public async Task<List<ExerciseDbDto>> GetByMuscleAsync(string muscle)
    {
        try
        {
            var muscleId = GetMuscleId(muscle);
            if (muscleId == 0) return new();

            var url = $"exercise/?language=2&limit=15&muscles={muscleId}";
            var response = await GetHttp().GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse<WgerExercise>>(json, JsonOptions);

            if (result?.Results == null || result.Results.Count == 0)
                return new();

            var exercises = new List<ExerciseDbDto>();
            foreach (var ex in result.Results)
            {
                var dto = new ExerciseDbDto
                {
                    Id = $"wger-{ex.Id}",
                    Name = ex.Name,
                    Category = ex.Category?.Name ?? "",
                    Equipment = ex.Equipment?.Select(e => e.Name).FirstOrDefault() ?? "",
                    PrimaryMuscles = ex.Muscles?.Select(m => m.Name).Where(n => n != null).Cast<string>().ToList() ?? new(),
                    Instructions = ex.Description != null ? new List<string> { ex.Description } : new(),
                    Images = new List<string>()
                };

                var imageUrl = await GetExerciseImageUrlAsync(ex.Id);
                if (!string.IsNullOrWhiteSpace(imageUrl))
                    dto.Images.Add(imageUrl);

                exercises.Add(dto);
            }

            return exercises;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Wger Muscle] ex: {ex.Message}");
            return new();
        }
    }

    private async Task<string?> GetExerciseImageUrlAsync(int exerciseId)
    {
        try
        {
            var url = $"exerciseimage/?exercise={exerciseId}&limit=1";
            var response = await GetHttp().GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WgerListResponse<WgerImage>>(json, JsonOptions);
            var img = result?.Results?.FirstOrDefault();
            if (img?.Image != null)
                return img.Image.StartsWith("http") ? img.Image : $"https://wger.de{img.Image}";
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static int GetMuscleId(string muscle)
    {
        return muscle.ToLowerInvariant() switch
        {
            "chest" or "petto" or "pectorals" => 4,
            "back" or "schiena" or "dorsals" => 12,
            "shoulders" or "spalle" or "deltoids" => 2,
            "biceps" or "bicipiti" => 1,
            "triceps" or "tricipiti" => 5,
            "abdominals" or "addominali" or "abs" => 6,
            "quadriceps" or "quadricipiti" or "quads" => 3,
            "hamstrings" or "femorali" => 11,
            "glutes" or "glutei" => 8,
            "calves" or "polpacci" => 15,
            _ => 0
        };
    }

    private class WgerListResponse<T>
    {
        public List<T> Results { get; set; } = new();
    }

    private class WgerExercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public WgerCategory? Category { get; set; }
        public List<WgerNameId>? Muscles { get; set; }
        public List<WgerNameId>? Equipment { get; set; }
    }

    private class WgerImage
    {
        public string? Image { get; set; }
    }

    private class WgerCategory
    {
        public string? Name { get; set; }
    }

    private class WgerNameId
    {
        public string? Name { get; set; }
    }
}
