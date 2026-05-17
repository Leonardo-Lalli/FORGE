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

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(token);
    public PocketBaseUserRecord? CurrentUser => currentUser;
    public string? Token => token;

    public string GetFileUrl(string collectionId, string recordId, string fileName)
    {
        var baseUrl = secrets.Get("POCKETBASE_URL")?.TrimEnd('/') ?? "";
        return $"{baseUrl}/api/files/{collectionId}/{recordId}/{fileName}?token={token}";
    }

    public PocketBaseService(HttpClient http, BuildSecrets secrets)
    {
        this.http = http;
        this.secrets = secrets;
    }

    private void EnsureInitialized()
    {
    }

    public void Initialize()
    {
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

    public async Task<(bool Success, string Error)> UpdateUserAsync(string? name = null, string? bio = null)
    {
        EnsureInitialized();
        if (currentUser == null || string.IsNullOrWhiteSpace(token))
            return (false, "Non autenticato.");

        try
        {
            var payload = new Dictionary<string, object?>();
            if (name != null) payload["name"] = name;
            if (bio != null) payload["bio"] = bio;

            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch,
                $"collections/users/records/{currentUser.Id}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return (false, ParseErrorMessage(body));
            }

            currentUser = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<bool> UploadAvatarAsync(Stream fileStream, string fileName)
    {
        if (currentUser == null || string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                fileName.EndsWith(".png") ? "image/png" : "image/jpeg");
            content.Add(streamContent, "avatar", fileName);

            var request = new HttpRequestMessage(HttpMethod.Patch,
                $"collections/users/records/{currentUser.Id}")
            {
                Content = content
            };
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            currentUser = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
            return true;
        }
        catch { return false; }
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

    public async Task<List<PocketBaseUserRecord>> SearchUsersAsync(string query)
    {
        if (!IsLoggedIn) return new();
        try
        {
            var url = $"collections/users/records?search={Uri.EscapeDataString(query)}&perPage=20";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[PB Search] status={response.StatusCode} body={await response.Content.ReadAsStringAsync()}");
                return new();
            }
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<PocketBaseUserRecord>>(JsonOptions);
            return list?.Items ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB Search] ex={ex.Message}");
            return new();
        }
    }

    public async Task<bool> SendFollowRequestAsync(string targetUserId)
    {
        if (!IsLoggedIn || currentUser == null) return false;
        try
        {
            var payload = new
            {
                from_user = currentUser.Id,
                from_name = currentUser.Name,
                to_user = targetUserId,
                status = "pending"
            };
            var (success, _) = await CreateRecordAsync("social_graph", payload);
            return success;
        }
        catch { return false; }
    }

    public async Task<List<SocialGraphRecord>> GetPendingRequestsAsync()
    {
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var filter = $"to_user=\"{currentUser.Id}\"&&status=\"pending\"";
            var url = $"collections/social_graph/records?filter={Uri.EscapeDataString(filter)}&perPage=50";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<SocialGraphRecord>>(JsonOptions);
            return list?.Items ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> AcceptFollowRequestAsync(string recordId)
    {
        if (!IsLoggedIn) return false;
        try
        {
            var payload = new { status = "accepted" };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/social_graph/records/{recordId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> RejectFollowRequestAsync(string recordId)
    {
        if (!IsLoggedIn) return false;
        try
        {
            var payload = new { status = "rejected" };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/social_graph/records/{recordId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<string>> GetFollowingUserIdsAsync()
    {
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var filter = $"from_user=\"{currentUser.Id}\"&&status=\"accepted\"";
            var url = $"collections/social_graph/records?filter={Uri.EscapeDataString(filter)}&perPage=200";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<SocialGraphRecord>>(JsonOptions);
            return list?.Items.Select(r => r.ToUser).ToList() ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<LoggedWorkoutRecord>> GetMyWorkoutsAsync(int limit = 10)
    {
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var filter = $"user=\"{currentUser.Id}\"";
            var url = $"collections/logged_workouts/records?filter={Uri.EscapeDataString(filter)}&perPage={limit}&sort=-created";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<LoggedWorkoutRecord>>(JsonOptions);
            return list?.Items ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<LoggedWorkoutRecord>> GetFollowedWorkoutsAsync()
    {
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var followingIds = await GetFollowingUserIdsAsync();
            if (followingIds.Count == 0) return new();

            var filterParts = followingIds.Select(id => $"user=\"{id}\"").ToList();
            var filter = string.Join("||", filterParts);
            var url = $"collections/logged_workouts/records?filter={Uri.EscapeDataString(filter)}&perPage=30&sort=-created";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();

            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<LoggedWorkoutRecord>>(JsonOptions);
            var records = list?.Items ?? new();

            foreach (var r in records)
            {
                var user = followingIds.Contains(r.User) ? r.User : "";
                if (!string.IsNullOrWhiteSpace(user))
                {
                    try
                    {
                        var userReq = new HttpRequestMessage(HttpMethod.Get, $"collections/users/records/{user}");
                        userReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var userRes = await http.SendAsync(userReq);
                        if (userRes.IsSuccessStatusCode)
                        {
                            var userRecord = await userRes.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
                            if (userRecord != null) r.UserName = userRecord.Name;
                        }
                    }
                    catch { }
                }
            }
            return records;
        }
        catch { return new(); }
    }

    public async Task<(bool Success, string Error)> LikeWorkoutAsync(string workoutId)
    {
        if (!IsLoggedIn || currentUser == null) return (false, "Non autenticato.");

        try
        {
            var getReq = new HttpRequestMessage(HttpMethod.Get, $"collections/logged_workouts/records/{workoutId}");
            getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var getRes = await http.SendAsync(getReq);
            if (!getRes.IsSuccessStatusCode) return (false, "Workout non trovato.");

            var record = await getRes.Content.ReadFromJsonAsync<LoggedWorkoutRecord>(JsonOptions);
            if (record == null) return (false, "Workout non trovato.");

            var likedBy = record.LikedBy ?? new List<string>();
            if (likedBy.Contains(currentUser.Id))
                return (true, string.Empty);

            likedBy.Add(currentUser.Id);
            var payload = new { liked_by = likedBy, likes = likedBy.Count };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/logged_workouts/records/{workoutId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            return response.IsSuccessStatusCode ? (true, string.Empty) : (false, "Errore like.");
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    public async Task<(bool Success, string Error)> UnlikeWorkoutAsync(string workoutId)
    {
        if (!IsLoggedIn || currentUser == null) return (false, "Non autenticato.");

        try
        {
            var getReq = new HttpRequestMessage(HttpMethod.Get, $"collections/logged_workouts/records/{workoutId}");
            getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var getRes = await http.SendAsync(getReq);
            if (!getRes.IsSuccessStatusCode) return (false, "Workout non trovato.");

            var record = await getRes.Content.ReadFromJsonAsync<LoggedWorkoutRecord>(JsonOptions);
            if (record == null) return (false, "Workout non trovato.");

            var likedBy = record.LikedBy ?? new List<string>();
            likedBy.Remove(currentUser.Id);
            var payload = new { liked_by = likedBy, likes = likedBy.Count };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/logged_workouts/records/{workoutId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await http.SendAsync(request);
            return response.IsSuccessStatusCode ? (true, string.Empty) : (false, "Errore unlike.");
        }
        catch (Exception ex) { return (false, ex.Message); }
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
