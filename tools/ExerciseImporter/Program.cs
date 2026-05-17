using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Config
const string PB_URL = "https://pocketbase.server-casa-leo.duckdns.org/api/";
const string EXERCISEDB_KEY = "YOUR_API_KEY"; // <-- metti la tua chiave ExerciseDB
const string PB_EMAIL = "YOUR_EMAIL";          // <-- metti la tua email
const string PB_PASSWORD = "YOUR_PASSWORD";    // <-- metti la tua password

if (EXERCISEDB_KEY == "YOUR_API_KEY")
{
    Console.WriteLine("ERRORE: devi impostare EXERCISEDB_KEY nel codice.");
    return;
}

var pbHttp = new HttpClient { BaseAddress = new Uri(PB_URL) };
var exHttp = new HttpClient
{
    BaseAddress = new Uri("https://exercise-db-fitness-workout-gym.p.rapidapi.com"),
    Timeout = TimeSpan.FromSeconds(30)
};
exHttp.DefaultRequestHeaders.Add("x-rapidapi-key", EXERCISEDB_KEY);
exHttp.DefaultRequestHeaders.Add("x-rapidapi-host", "exercise-db-fitness-workout-gym.p.rapidapi.com");

var redirectHttp = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true })
{
    Timeout = TimeSpan.FromSeconds(10)
};
redirectHttp.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

// Login to PocketBase
Console.WriteLine("[1/4] Login PocketBase...");
var authPayload = new { identity = PB_EMAIL, password = PB_PASSWORD };
var authResp = await pbHttp.PostAsJsonAsync("collections/users/auth-with-password", authPayload);
if (!authResp.IsSuccessStatusCode)
{
    Console.WriteLine($"ERRORE login: {await authResp.Content.ReadAsStringAsync()}");
    return;
}
var auth = await authResp.Content.ReadFromJsonAsync<AuthResponse>();
var token = auth?.Token ?? "";
Console.WriteLine($"  OK, token={token[..20]}...");

// Get all exercise IDs
Console.WriteLine("[2/4] Fetching all exercise IDs...");
var idsResp = await exHttp.GetAsync("/exercises");
idsResp.EnsureSuccessStatusCode();
var idsJson = await idsResp.Content.ReadAsStringAsync();
var idsWrapper = JsonSerializer.Deserialize<ExerciseListResponse>(idsJson);
var ids = idsWrapper?.ExerciseIds ?? new();
Console.WriteLine($"  Found {ids.Count} exercises");

// Resolve and cache images
Console.WriteLine("[3/4] Resolving image URLs and caching to PocketBase...");
var cached = new HashSet<string>();
int resolved = 0;

foreach (var id in ids.Take(200)) // Limit to 200 to avoid API rate limits
{
    try
    {
        var detailResp = await exHttp.GetAsync($"/exercise/{Uri.EscapeDataString(id)}");
        if (!detailResp.IsSuccessStatusCode) continue;
        var detail = await detailResp.Content.ReadFromJsonAsync<ExerciseDetail>();

        var shortImg = detail?.Images?.FirstOrDefault() ?? "";
        if (string.IsNullOrWhiteSpace(shortImg)) continue;

        // Resolve short URL
        string resolvedUrl = shortImg;
        if (shortImg.Contains("acesse.dev") || shortImg.Contains("encr.pw") || shortImg.Contains("l1nq.com"))
        {
            try
            {
                var fullUrl = shortImg.StartsWith("http") ? shortImg : $"https://{shortImg}";
                var redirectResp = await redirectHttp.GetAsync(fullUrl);
                resolvedUrl = redirectResp.RequestMessage?.RequestUri?.ToString() ?? fullUrl;
                Console.WriteLine($"  [{id}] {shortImg} -> {resolvedUrl[..Math.Min(resolvedUrl.Length, 60)]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [{id}] SKIP: {ex.Message}");
                resolvedUrl = $"https://{shortImg}";
            }
        }

        // Cache to PocketBase
        var payload = new Dictionary<string, object>
        {
            ["name"] = detail?.Name ?? "",
            ["bodyPart"] = detail?.PrimaryMuscles?.FirstOrDefault() ?? "",
            ["equipment"] = detail?.Equipment ?? "",
            ["instructions"] = detail?.Instructions ?? new List<string>(),
            ["imageUrl"] = resolvedUrl,
            ["category"] = detail?.Category ?? "",
            ["level"] = detail?.Level ?? "",
            ["force"] = detail?.Force ?? "",
            ["mechanic"] = detail?.Mechanic ?? ""
        };

        var json = JsonSerializer.Serialize(payload);
        var req = new HttpRequestMessage(HttpMethod.Post, "collections/excercise/records")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var pbResp = await pbHttp.SendAsync(req);

        if (pbResp.IsSuccessStatusCode)
        {
            resolved++;
            if (resolved % 20 == 0) Console.WriteLine($"  ... {resolved} cached so far");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  [{id}] ERROR: {ex.Message}");
    }
}

Console.WriteLine($"[4/4] Done! {resolved} exercises cached to PocketBase.");

// DTOs
class AuthResponse
{
    [JsonPropertyName("token")] public string Token { get; set; } = "";
    [JsonPropertyName("record")] public Record? Record { get; set; }
}
class Record
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
}
class ExerciseListResponse
{
    [JsonPropertyName("exercises")] public List<string> ExerciseIds { get; set; } = new();
}
class ExerciseDetail
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("images")] public List<string>? Images { get; set; }
    [JsonPropertyName("primaryMuscles")] public List<string>? PrimaryMuscles { get; set; }
    [JsonPropertyName("equipment")] public string? Equipment { get; set; }
    [JsonPropertyName("instructions")] public List<string>? Instructions { get; set; }
    [JsonPropertyName("category")] public string? Category { get; set; }
    [JsonPropertyName("level")] public string? Level { get; set; }
    [JsonPropertyName("force")] public string? Force { get; set; }
    [JsonPropertyName("mechanic")] public string? Mechanic { get; set; }
}
