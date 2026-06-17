using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Config
const string PB_URL = "https://leoforge.duckdns.org/api/";
var PB_EMAIL = Environment.GetEnvironmentVariable("FORGE_PB_EMAIL") ?? "";
var PB_PASSWORD = Environment.GetEnvironmentVariable("FORGE_PB_PASSWORD") ?? "";
var PB_IS_ADMIN = Environment.GetEnvironmentVariable("FORGE_PB_ADMIN") == "1";

if (string.IsNullOrWhiteSpace(PB_EMAIL))
{
    Console.WriteLine("ERRORE: imposta le variabili d'ambiente:");
    Console.WriteLine("  Per utente PocketBase normale:");
    Console.WriteLine("    $env:FORGE_PB_EMAIL='tua_email'");
    Console.WriteLine("    $env:FORGE_PB_PASSWORD='tua_password'");
    Console.WriteLine("  Per admin PocketBase:");
    Console.WriteLine("    $env:FORGE_PB_EMAIL='admin_email'");
    Console.WriteLine("    $env:FORGE_PB_PASSWORD='admin_password'");
    Console.WriteLine("    $env:FORGE_PB_ADMIN='1'");
    return;
}

var pbHttp = new HttpClient { BaseAddress = new Uri(PB_URL), Timeout = TimeSpan.FromSeconds(30) };

var exHttp = new HttpClient
{
    BaseAddress = new Uri("https://oss.exercisedb.dev/api/v1/"),
    Timeout = TimeSpan.FromSeconds(30)
};

// Login PocketBase (admin o utente normale)
var authEndpoint = PB_IS_ADMIN
    ? "collections/_superusers/auth-with-password"
    : "collections/users/auth-with-password";

Console.WriteLine($"[1/3] Login PocketBase ({(PB_IS_ADMIN ? "admin" : "user")})...");
var authPayload = new { identity = PB_EMAIL, password = PB_PASSWORD };
var authResp = await pbHttp.PostAsJsonAsync(authEndpoint, authPayload);
if (!authResp.IsSuccessStatusCode)
{
    Console.WriteLine($"ERRORE login: {await authResp.Content.ReadAsStringAsync()}");
    return;
}
var auth = await authResp.Content.ReadFromJsonAsync<AuthResponse>();
var token = auth?.Token ?? "";
var userId = auth?.Record?.Id ?? auth?.Admin?.Id ?? "?";
Console.WriteLine($"  OK, id={userId}");

// Resume file
var resumeFile = Path.Combine(Directory.GetCurrentDirectory(), "import_resume.txt");
string cursor = "";
int skipCount = 0;

if (File.Exists(resumeFile))
{
    var lines = File.ReadAllLines(resumeFile);
    if (lines.Length >= 2)
    {
        cursor = lines[0];
        skipCount = int.Parse(lines[1]);
        Console.WriteLine($"  Resume: cursor={cursor[..Math.Min(cursor.Length,30)]} skip={skipCount}");
    }
}

Console.WriteLine($"[2/3] Importing exercises to PocketBase (resume after {skipCount})...");

int imported = skipCount;
int pageCount = 0;
int totalPages = 0;

while (true)
{
    pageCount++;
    var url = $"exercises?limit=100{(string.IsNullOrEmpty(cursor) ? "" : $"&cursor={cursor}")}";
    Console.Write($"  Page {pageCount}: GET {url[..Math.Min(url.Length,60)]}... ");

    HttpResponseMessage response;
    try
    {
        response = await exHttp.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n  ERROR fetching page {pageCount}: {ex.Message}");
        break;
    }

    var json = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ExerciseDbResponse>(json);
    if (result?.Data == null || result.Data.Count == 0)
    {
        Console.WriteLine("empty, done!");
        break;
    }

    foreach (var ex in result.Data)
    {
        if (string.IsNullOrWhiteSpace(ex.ExerciseId)) continue;
        if (string.IsNullOrWhiteSpace(ex.GifUrl))
        {
            Console.Write("S");
            continue;
        }

        try
        {
            var payload = new Dictionary<string, object>
            {
                ["exercise_id"] = ex.ExerciseId,
                ["name"] = Capitalize(ex.Name),
                ["bodyPart"] = ex.GetBodyParts().FirstOrDefault() ?? "",
                ["equipment"] = ex.GetEquipments().FirstOrDefault() ?? "",
                ["instructions"] = ex.GetInstructions(),
                ["imageUrl"] = ex.GifUrl,
                ["category"] = ex.GetBodyParts().FirstOrDefault() ?? "",
                ["targetMuscles"] = string.Join(",", ex.GetTargetMuscles()),
                ["secondaryMuscles"] = string.Join(",", ex.GetSecondaryMuscles())
            };

            var payloadJson = JsonSerializer.Serialize(payload);
            var req = new HttpRequestMessage(HttpMethod.Post, "collections/excercise/records")
            {
                Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var pbResp = await pbHttp.SendAsync(req);

            if (pbResp.IsSuccessStatusCode)
            {
                imported++;
                if (imported % 50 == 0) Console.WriteLine($"\n    ... {imported} imported so far");
            }
            else
            {
                var err = await pbResp.Content.ReadAsStringAsync();
                if (err.Contains("duplicate") || err.Contains("already exists"))
                    Console.Write("D");
                else
                    Console.Write("F");
            }
        }
        catch (Exception exc)
        {
            Console.Write("E");
            System.Diagnostics.Debug.WriteLine($"  ERR [{ex.ExerciseId}]: {exc.Message}");
        }

        await Task.Delay(100); // 10 req/s to avoid rate limiting
    }

    Console.Write($" ({imported} total)");

    if (!string.IsNullOrEmpty(result.Meta?.NextCursor) && result.Meta.HasNextPage)
    {
        cursor = result.Meta.NextCursor;
        File.WriteAllLines(resumeFile, new[] { cursor, imported.ToString() });
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("\n  No more pages, done!");
        break;
    }

    totalPages = pageCount;
}

// Cleanup resume file
if (File.Exists(resumeFile))
    File.Delete(resumeFile);

Console.WriteLine($"[3/3] Import completato: {imported} esercizi in {totalPages} pagine.");

static string Capitalize(string name)
{
    if (string.IsNullOrWhiteSpace(name)) return name;
    return string.Join(" ", name.Split(' ').Select(w =>
        w.Length > 0 ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
}

// DTOs
class AuthResponse
{
    [JsonPropertyName("token")] public string Token { get; set; } = "";
    [JsonPropertyName("record")] public AuthRecord? Record { get; set; }
    [JsonPropertyName("admin")] public AuthRecord? Admin { get; set; }
}
class AuthRecord { [JsonPropertyName("id")] public string Id { get; set; } = ""; }

class ExerciseDbResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("data")] public List<ExerciseDbItem> Data { get; set; } = new();
    [JsonPropertyName("meta")] public ExerciseDbMeta? Meta { get; set; }
}

class ExerciseDbMeta
{
    [JsonPropertyName("total")] public int Total { get; set; }
    [JsonPropertyName("hasNextPage")] public bool HasNextPage { get; set; }
    [JsonPropertyName("nextCursor")] public string NextCursor { get; set; } = "";
}

class ExerciseDbItem
{
    [JsonPropertyName("exerciseId")] public string ExerciseId { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("gifUrl")] public string GifUrl { get; set; } = "";
    [JsonPropertyName("bodyParts")] public object? BodyPartsRaw { get; set; }
    [JsonPropertyName("equipments")] public object? EquipmentsRaw { get; set; }
    [JsonPropertyName("targetMuscles")] public object? TargetMusclesRaw { get; set; }
    [JsonPropertyName("secondaryMuscles")] public object? SecondaryMusclesRaw { get; set; }
    [JsonPropertyName("instructions")] public object? InstructionsRaw { get; set; }

    public List<string> GetBodyParts() => NormalizeList(BodyPartsRaw);
    public List<string> GetEquipments() => NormalizeList(EquipmentsRaw);
    public List<string> GetTargetMuscles() => NormalizeList(TargetMusclesRaw);
    public List<string> GetSecondaryMuscles() => NormalizeList(SecondaryMusclesRaw, " ");
    public List<string> GetInstructions() => NormalizeList(InstructionsRaw);

    private static List<string> NormalizeList(object? raw, string? splitSep = null)
    {
        if (raw == null) return new();
        if (raw is JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array)
                return el.EnumerateArray().Select(e => e.GetString() ?? "").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (el.ValueKind == JsonValueKind.String)
            {
                var str = el.GetString() ?? "";
                return splitSep != null
                    ? str.Split(splitSep, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string> { str };
            }
        }
        return new();
    }
}
