using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GymTracker.Mobile.Models.Dto;

namespace GymTracker.Mobile.Services;

public class PocketBaseService
{
    private readonly HttpClient http;
    private readonly BuildSecrets secrets;
    private string? token;
    private PocketBaseUserRecord? currentUser;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private bool initialized;

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(token);
    public PocketBaseUserRecord? CurrentUser => currentUser;

    public PocketBaseService(HttpClient http, BuildSecrets secrets)
    {
        this.http = http;
        this.secrets = secrets;
    }

    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;
        var baseUrl = secrets.Get("POCKETBASE_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl))
            http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/api/");
    }

    public void Initialize()
    {
        EnsureInitialized();
    }

    public async Task<(bool Success, string Error)> LoginAsync(string email, string password)
    {
        EnsureInitialized();
        try
        {
            var payload = new { identity = email, password };
            var response = await http.PostAsJsonAsync("collections/users/auth-with-password", payload, JsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = ParseErrorMessage(errorBody);
                return (false, errorMsg);
            }

            var auth = await response.Content.ReadFromJsonAsync<PocketBaseAuthResponse>(JsonOptions);
            if (auth == null)
                return (false, "Risposta non valida dal server.");

            token = auth.Token;
            currentUser = auth.Record;
            SaveCredentials(email, password);

            return (true, string.Empty);
        }
        catch (HttpRequestException)
        {
            return (false, "Impossibile connettersi al server PocketBase.");
        }
        catch (TaskCanceledException)
        {
            return (false, "Timeout di connessione.");
        }
        catch (Exception ex)
        {
            return (false, $"Errore: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Error)> RegisterAsync(string email, string password, string name)
    {
        EnsureInitialized();
        try
        {
            var payload = new
            {
                email,
                password,
                passwordConfirm = password,
                name
            };
            var response = await http.PostAsJsonAsync("collections/users/records", payload, JsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorMsg = ParseErrorMessage(errorBody);
                return (false, errorMsg);
            }

            return await LoginAsync(email, password);
        }
        catch (HttpRequestException)
        {
            return (false, "Impossibile connettersi al server PocketBase.");
        }
        catch (Exception ex)
        {
            return (false, $"Errore: {ex.Message}");
        }
    }

    public void Logout()
    {
        token = null;
        currentUser = null;
        Preferences.Remove("pb_email");
        Preferences.Remove("pb_password");
    }

    public async Task<(bool Success, string Error)> RefreshUserAsync()
    {
        EnsureInitialized();
        if (currentUser == null || string.IsNullOrWhiteSpace(token))
            return (false, "Non autenticato.");

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"collections/users/records/{currentUser.Id}");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return (false, "Impossibile recuperare il profilo.");

            currentUser = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Error)> GetListAsync<T>(string collection, string? filter = null)
    {
        EnsureInitialized();
        try
        {
            var url = $"collections/{collection}/records";
            if (!string.IsNullOrWhiteSpace(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return (false, ParseErrorMessage(body));
            }

            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<T>>(JsonOptions);
            return list != null ? (true, string.Empty) : (false, "Nessun dato.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Error)> CreateRecordAsync<T>(string collection, T record)
    {
        EnsureInitialized();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"collections/{collection}/records")
            {
                Content = new StringContent(JsonSerializer.Serialize(record, JsonOptions),
                    Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return (false, ParseErrorMessage(body));
            }
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        var email = Preferences.Get("pb_email", string.Empty);
        var password = Preferences.Get("pb_password", string.Empty);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        var (success, _) = await LoginAsync(email, password);
        return success;
    }

    private void SaveCredentials(string email, string password)
    {
        Preferences.Set("pb_email", email);
        Preferences.Set("pb_password", password);
    }

    private static string ParseErrorMessage(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var msg))
                return msg.GetString() ?? "Errore sconosciuto.";

            if (doc.RootElement.TryGetProperty("data", out var data) &&
                data.ValueKind == JsonValueKind.Object)
            {
                var messages = new List<string>();
                foreach (var prop in data.EnumerateObject())
                {
                    if (prop.Value.TryGetProperty("message", out var fieldMsg))
                        messages.Add(fieldMsg.GetString() ?? "");
                }
                if (messages.Count > 0)
                    return string.Join(" ", messages);
            }
        }
        catch { }

        return body.Length > 200 ? body[..200] : body;
    }
}
