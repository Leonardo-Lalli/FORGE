using System.Text;
using System.Text.Json;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class ExerciseApiService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly BuildSecrets secrets;
    private readonly PocketBaseService? pb;
    private List<string>? cachedIds;
    private readonly HashSet<string> pbCacheCheck = new();
    private HttpClient? _http;
    private HttpClient? _redirectHttp;

    private HttpClient GetHttp()
    {
        if (_http != null) return _http;
        _http = httpFactory.CreateClient("exercisedb");
        _http.Timeout = TimeSpan.FromSeconds(10);
        return _http;
    }

    private HttpClient GetRedirectHttp()
    {
        if (_redirectHttp != null) return _redirectHttp;
        _redirectHttp = httpFactory.CreateClient("redirect");
        return _redirectHttp;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ExerciseApiService(IHttpClientFactory httpFactory, BuildSecrets secrets, PocketBaseService pb)
    {
        this.httpFactory = httpFactory;
        this.secrets = secrets;
        this.pb = pb;
    }

    public void Initialize()
    {
        var apiKey = secrets.Get("EXERCISEDB_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey)) return;

        var http = GetHttp();
        GetHttp().BaseAddress = new Uri("https://exercise-db-fitness-workout-gym.p.rapidapi.com");
        GetHttp().DefaultRequestHeaders.Clear();
        GetHttp().DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
        GetHttp().DefaultRequestHeaders.Add("x-rapidapi-host", "exercise-db-fitness-workout-gym.p.rapidapi.com");
    }

    public async Task<List<string>> GetAllExerciseIdsAsync()
    {
        if (cachedIds != null) return cachedIds;

        try
        {
            var response = await GetHttp().GetAsync("/exercises");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<ExerciseListResponse>(json, JsonOptions);
            cachedIds = list?.ExerciseIds ?? new();
            return cachedIds;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseApi] GetAllIds failed: {ex.Message}");
            return new();
        }
    }

    public async Task<string?> GetCachedImageUrlAsync(string exerciseName)
    {
        try
        {
            if (pb == null) return null;
            return await pb.GetCachedExerciseImageAsync(exerciseName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseApi CacheImg] ex: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ExerciseDbDto>> SearchByNameAsync(string name)
    {
        var searchTerm = name.ToLowerInvariant().Replace(" ", "_");

        var allIds = await GetAllExerciseIdsAsync();

        var matches = allIds
            .Where(id => id.ToLowerInvariant().Contains(searchTerm))
            .Take(10)
            .ToList();

        if (matches.Count == 0) return new();

        var tasks = matches.Select(async id =>
        {
            try
            {
                var response = await GetHttp().GetAsync($"/exercise/{Uri.EscapeDataString(id)}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<ExerciseDbDto>(json, JsonOptions);
                if (detail != null)
                {
                    // Resolve image URL
                    var shortImg = detail.Images.FirstOrDefault() ?? "";
                    var resolved = await ResolveImageUrlAsync(shortImg);
                    if (!string.IsNullOrWhiteSpace(resolved) && detail.Images.Count == 0)
                        detail.Images.Add(resolved);
                    else if (!string.IsNullOrWhiteSpace(resolved) && detail.Images.Count > 0)
                        detail.Images[0] = resolved;

                    await CacheExerciseAsync(detail, resolved);
                }
                return detail;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExerciseApi Search] id ex: {ex.Message}");
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null).Cast<ExerciseDbDto>().ToList();
    }

    private async Task<string> ResolveImageUrlAsync(string shortUrl)
    {
        if (string.IsNullOrWhiteSpace(shortUrl)) return string.Empty;
        if (!shortUrl.Contains("acesse.dev") && !shortUrl.Contains("encr.pw") && !shortUrl.Contains("l1nq.com"))
            return shortUrl.StartsWith("http") ? shortUrl : $"https://{shortUrl}";

        try
        {
            var fullUrl = shortUrl.StartsWith("http") ? shortUrl : $"https://{shortUrl}";
            var response = await GetRedirectHttp().GetAsync(fullUrl);
            return response.RequestMessage?.RequestUri?.ToString() ?? fullUrl;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseApi ResolveImg] ex: {ex.Message}");
            return $"https://{shortUrl}";
        }
    }

    private async Task CacheExerciseAsync(ExerciseDbDto exercise, string resolvedImageUrl)
    {
        if (pb == null || !pb.IsLoggedIn) return;
        if (pbCacheCheck.Contains(exercise.Id)) return;

        try
        {
            pbCacheCheck.Add(exercise.Id);

            var payload = new Dictionary<string, object?>
            {
                ["name"] = exercise.Name,
                ["bodyPart"] = exercise.PrimaryMuscles.FirstOrDefault() ?? "",
                ["equipment"] = exercise.Equipment ?? "",
                ["instructions"] = exercise.Instructions ?? new List<string>(),
                ["imageUrl"] = resolvedImageUrl,
                ["category"] = exercise.Category ?? "",
                ["level"] = exercise.Level ?? "",
                ["force"] = exercise.Force ?? "",
                ["mechanic"] = exercise.Mechanic ?? ""
            };
            var (ok, err) = await pb.CreateRecordAsync("excercise", payload);
            if (!ok)
                System.Diagnostics.Debug.WriteLine($"[CacheExercise] pb error: {err}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CacheExercise] ex: {ex.Message}");
        }
    }

    public async Task<ExerciseDbDto?> GetExerciseByIdAsync(string exerciseId)
    {
        try
        {
            var response = await GetHttp().GetAsync($"/exercise/{Uri.EscapeDataString(exerciseId)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ExerciseDbDto>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseApi] GetById failed: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ExerciseDbDto>> GetByMuscleAsync(string muscle)
    {
        try
        {
            var response = await GetHttp().GetAsync($"/exercises/muscle/{Uri.EscapeDataString(muscle)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var exercises = JsonSerializer.Deserialize<List<ExerciseDbDto>>(json, JsonOptions) ?? new();

            // Resolve image URLs for first 15 results before returning
            var first = exercises.Take(15).ToList();
            var tasks = first.Select(async ex =>
            {
                var shortImg = ex.Images.FirstOrDefault() ?? "";
                var resolved = await ResolveImageUrlAsync(shortImg);
                if (!string.IsNullOrWhiteSpace(resolved))
                {
                    if (ex.Images.Count == 0) ex.Images.Add(resolved);
                    else ex.Images[0] = resolved;
                }
                await CacheExerciseAsync(ex, resolved);
            });
            _ = Task.WhenAll(tasks).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                    System.Diagnostics.Debug.WriteLine($"[ExerciseApi MuscCache] ex: {t.Exception.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);

            return exercises;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExerciseApi] GetByMuscle failed: {ex.Message}");
            return new();
        }
    }
}
