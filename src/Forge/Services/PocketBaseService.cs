using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Forge.Models.Dto;

namespace Forge.Services;

public class PocketBaseService
{
    private readonly IHttpClientFactory httpFactory;
    private readonly BuildSecrets secrets;
    private string? token;
    private PocketBaseUserRecord? currentUser;
    private HttpClient? _http;

    private string GetPbBaseUrl()
    {
        var configured = Preferences.Get("pb_server_url", string.Empty);
        if (!string.IsNullOrWhiteSpace(configured))
            return configured.TrimEnd('/');

        return secrets.Get("POCKETBASE_URL")?.TrimEnd('/') ?? "";
    }

    private HttpClient GetHttp()
    {
        if (_http != null) return _http;
        var client = httpFactory.CreateClient("pocketbase");
        var pbUrl = GetPbBaseUrl();
        if (string.IsNullOrWhiteSpace(pbUrl))
            throw new InvalidOperationException("Server URL not configured. Go to Impostazioni and set the PocketBase URL.");
        client.BaseAddress = new Uri($"{pbUrl}/api/");
        client.Timeout = TimeSpan.FromSeconds(15);
        _http = client;
        return _http;
    }

    public void InvalidateClient()
    {
        _http?.Dispose();
        _http = null;
        token = null;
        currentUser = null;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(token);
    public PocketBaseUserRecord? CurrentUser => currentUser;
    internal string? Token => token;

    public string GetFileUrl(string collectionId, string recordId, string fileName)
    {
        var pbUrl = GetPbBaseUrl();
        var coll = string.IsNullOrWhiteSpace(collectionId) ? "users" : collectionId;
        var url = $"{pbUrl}/api/files/{coll}/{recordId}/{fileName}";
        if (!string.IsNullOrWhiteSpace(token))
            url += $"?token={Uri.EscapeDataString(token)}";
        return url;
    }

    public async Task<ImageSource?> DownloadAvatarAsync(string collectionId, string recordId, string fileName)
    {
        await EnsureAuthAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var coll = string.IsNullOrWhiteSpace(collectionId) ? "users" : collectionId;
            var url = $"{GetPbBaseUrl()}/api/files/{coll}/{recordId}/{fileName}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[PB DownloadAvatar] HTTP {response.StatusCode} for {url}");
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            if (bytes.Length == 0) return null;

            System.Diagnostics.Debug.WriteLine($"[PB DownloadAvatar] OK {bytes.Length} bytes from {fileName}");
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB DownloadAvatar] ex: {ex.Message}");
            return null;
        }
    }

    public PocketBaseService(IHttpClientFactory httpFactory, BuildSecrets secrets)
    {
        this.httpFactory = httpFactory;
        this.secrets = secrets;
    }

    private async Task EnsureAuthAsync()
    {
        if (!string.IsNullOrWhiteSpace(token)) return;
        await TryAutoLoginAsync();
    }

    private async Task<bool> TryHandle401Async(HttpResponseMessage response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            return false;
        if (string.IsNullOrWhiteSpace(token))
            return false;

        System.Diagnostics.Debug.WriteLine("[PB] 401 received, attempting token refresh...");
        var refreshed = await RefreshTokenAsync();
        if (refreshed)
            System.Diagnostics.Debug.WriteLine("[PB] Token refreshed successfully after 401.");
        else
            System.Diagnostics.Debug.WriteLine("[PB] Token refresh failed after 401.");
        return refreshed;
    }

    private async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        try
        {
            var payload = new { };
            var response = await GetHttp().PostAsJsonAsync("collections/users/auth-refresh", payload, JsonOptions);
            if (!response.IsSuccessStatusCode) return false;
            var auth = await response.Content.ReadFromJsonAsync<PocketBaseAuthResponse>(JsonOptions);
            if (auth == null || string.IsNullOrWhiteSpace(auth.Token)) return false;
            token = auth.Token;
            if (auth.Record != null) currentUser = auth.Record;
            System.Diagnostics.Debug.WriteLine("[PB] Token refreshed");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB RefreshToken] ex: {ex.Message}");
            return false;
        }
    }

    public async Task<(bool Success, string Error)> LoginAsync(string email, string password)
    {
        
        try
        {
            var payload = new { identity = email, password };
            var response = await GetHttp().PostAsJsonAsync("collections/users/auth-with-password", payload, JsonOptions);

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
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[PB Login] userId={currentUser.Id}");
#endif
            await SaveCredentialsAsync(email, password);

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
            System.Diagnostics.Debug.WriteLine($"[PB Login] ex: {ex.Message}"); return (false, "Errore di autenticazione.");
        }
    }

    public async Task<(bool Success, string Error)> RegisterAsync(string email, string password, string name)
    {
        
        try
        {
            var payload = new
            {
                email,
                password,
                passwordConfirm = password,
                name
            };
            var response = await GetHttp().PostAsJsonAsync("collections/users/records", payload, JsonOptions);

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
            System.Diagnostics.Debug.WriteLine($"[PB Login] ex: {ex.Message}"); return (false, "Errore di autenticazione.");
        }
    }

    public void Logout()
    {
        token = null;
        currentUser = null;
        Preferences.Remove("pb_email");
        Preferences.Remove("pb_password");
        try { SecureStorage.Remove("pb_password"); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB Logout] SecureStorage remove err: {ex.Message}"); }
        try { SecureStorage.Remove("pb_email"); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB Logout] SecureStorage email err: {ex.Message}"); }
    }

    public async Task<(bool Success, string Error)> RefreshUserAsync()
    {
        await EnsureAuthAsync();
        if (currentUser == null || string.IsNullOrWhiteSpace(token))
            return (false, "Non autenticato.");

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"collections/users/records/{currentUser.Id}");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return (false, "Impossibile recuperare il profilo.");

            currentUser = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server.");
        }
    }

    public async Task UpdateFcmTokenAsync(string fcmToken)
    {
        if (!IsLoggedIn || string.IsNullOrWhiteSpace(fcmToken) || currentUser == null) return;
        try
        {
            var payload = new Dictionary<string, object> { ["fcm_token"] = fcmToken };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Patch,
                $"collections/users/records/{currentUser.Id}")
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await GetHttp().SendAsync(request);
            System.Diagnostics.Debug.WriteLine($"[PB FcmToken] updated");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB FcmToken] ex: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Error)> UpdateUserAsync(string? name = null, string? bio = null)
    {
        await EnsureAuthAsync();
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

            var response = await GetHttp().SendAsync(request);
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
            System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server.");
        }
    }

    public async Task<bool> UploadAvatarAsync(Stream fileStream, string fileName)
    {
        await EnsureAuthAsync();
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

            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            currentUser = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
            return true;
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[PB UploadAvatar] ex: {ex.Message}");
#endif
            return false;
        }
    }

    public async Task<(bool Success, string Error)> GetListAsync<T>(string collection, string? filter = null)
    {
        
        try
        {
            var url = $"collections/{collection}/records";
            if (!string.IsNullOrWhiteSpace(filter))
                url += $"?filter={Uri.EscapeDataString(filter)}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await GetHttp().SendAsync(request);
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
            System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server.");
        }
    }

    public async Task<(bool Success, string Error)> CreateRecordAsync<T>(string collection, T record)
    {
        
        try
        {
            var json = JsonSerializer.Serialize(record, JsonOptions);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[PB Create] collection={collection} auth={!string.IsNullOrWhiteSpace(token)}");
#endif
            var request = new HttpRequestMessage(HttpMethod.Post, $"collections/{collection}/records")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await GetHttp().SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
#if DEBUG
            if (!response.IsSuccessStatusCode)
                System.Diagnostics.Debug.WriteLine($"[PB Create] status={response.StatusCode} err={body[..Math.Min(body.Length, 50)]}");
#endif
            if (!response.IsSuccessStatusCode)
            {
                return (false, ParseErrorMessage(body));
            }
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[PB Create] EX: {ex.Message}");
#endif
            System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server.");
        }
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        string? email = null;
        try { email = await SecureStorage.GetAsync("pb_email"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB AutoLogin] SecureStorage email err: {ex.Message}"); }

        if (string.IsNullOrWhiteSpace(email))
        {
            email = Preferences.Get("pb_email", string.Empty);
            if (!string.IsNullOrWhiteSpace(email))
            {
                try { await SecureStorage.SetAsync("pb_email", email); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB AutoLogin] SecureStorage email save err: {ex.Message}"); }
                Preferences.Remove("pb_email");
            }
        }
        if (string.IsNullOrWhiteSpace(email)) return false;

        string? password = null;
        try { password = await SecureStorage.GetAsync("pb_password"); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB AutoLogin] SecureStorage pw err: {ex.Message}"); }

        if (string.IsNullOrWhiteSpace(password))
        {
            password = Preferences.Get("pb_password", string.Empty);
            if (!string.IsNullOrWhiteSpace(password))
            {
                try { await SecureStorage.SetAsync("pb_password", password); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB AutoLogin] SecureStorage pw save err: {ex.Message}"); }
                Preferences.Remove("pb_password");
            }
        }
        if (string.IsNullOrWhiteSpace(password)) return false;

        var (success, _) = await LoginAsync(email, password);
        return success;
    }

    private async Task SaveCredentialsAsync(string email, string password)
    {
        try { await SecureStorage.SetAsync("pb_email", email); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB] SecureStorage email err: {ex.Message}"); }
        try
        {
            await SecureStorage.SetAsync("pb_password", password);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB] SecureStorage unavailable: {ex.Message}. Auto-login disabled.");
        }
    }

    public async Task<List<PocketBaseUserRecord>> SearchUsersAsync(string query)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return new();
        try
        {
            var url = $"collections/users/records?search={Uri.EscapeDataString(query)}&perPage=20";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[PB Search] status={response.StatusCode} body={await response.Content.ReadAsStringAsync()}");
#endif
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
        await EnsureAuthAsync();
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB SendFollow] ex: {ex.Message}");
            return false;
        }
    }

    public async Task<List<SocialGraphRecord>> GetPendingRequestsAsync()
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var url = "collections/social_graph/records?perPage=50";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<SocialGraphRecord>>(JsonOptions);
            return list?.Items
                .Where(r => r.ToUser == currentUser.Id && r.Status == "pending")
                .ToList() ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB PendingReq] ex: {ex.Message}");
            return new();
        }
    }

    public async Task<bool> AcceptFollowRequestAsync(string recordId)
    {
        await EnsureAuthAsync();
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
            var response = await GetHttp().SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB AcceptReq] ex: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RejectFollowRequestAsync(string recordId)
    {
        await EnsureAuthAsync();
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
            var response2 = await GetHttp().SendAsync(request);
            return response2.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB RejectReq] ex: {ex.Message}");
            return false;
        }
    }

    public async Task<List<string>> GetFollowingUserIdsAsync()
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var url = "collections/social_graph/records?perPage=200";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();
            var list = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<SocialGraphRecord>>(JsonOptions);
            return list?.Items
                .Where(r => r.FromUser == currentUser.Id && r.Status == "accepted")
                .Select(r => r.ToUser)
                .ToList() ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB FollowingIds] ex: {ex.Message}");
            return new();
        }
    }

    public async Task<List<LoggedWorkoutRecord>> GetMyWorkoutsAsync(int limit = 10)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null)
        {
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] skipped: IsLoggedIn={IsLoggedIn} currentUser={currentUser != null}");
            return new();
        }
        try
        {
            var userId = currentUser.Id;
            var cappedLimit = Math.Min(limit, 200);

            var result = await TryFetchWorkoutsAsync(cappedLimit, userId);
            if (result != null)
            {
                result = result.OrderByDescending(w =>
                {
                    DateTime.TryParse(w.Date, out var d);
                    return d;
                }).ToList();
            }
            return result ?? new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] ex={ex}");
            return new();
        }
    }

    private async Task<List<LoggedWorkoutRecord>?> TryFetchWorkoutsAsync(int perPage, string userId)
    {
        try
        {
            var url = $"collections/logged_workouts/records?perPage={perPage}";
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] url={url}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await TryHandle401Async(response))
                {
                    request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    response = await GetHttp().SendAsync(request);
                }
            }
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] status={response.StatusCode} bodyLen={body.Length}");
            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] FAIL body[..200]={body[..Math.Min(body.Length, 200)]}");
#endif
                return null;
            }
            return ParseWorkoutRecords(body, userId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] TryFetch err: {ex.Message}");
            return null;
        }
    }

    public async Task<LoggedWorkoutRecord?> GetWorkoutByIdAsync(string workoutId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return null;
        try
        {
            var url = $"collections/logged_workouts/records/{Uri.EscapeDataString(workoutId)}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[PB WorkoutById] status={response.StatusCode}");
                return null;
            }
            var records = ParseWorkoutRecordsFromSingleItem(body);
            return records.FirstOrDefault();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB WorkoutById] ex={ex.Message}");
            return null;
        }
    }

    private List<LoggedWorkoutRecord> ParseWorkoutRecordsFromSingleItem(string body)
    {
        var results = new List<LoggedWorkoutRecord>();
        try
        {
            using var doc = JsonDocument.Parse(body);
            var item = doc.RootElement;
            var record = ParseWorkoutItem(item);
            if (record != null) results.Add(record);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB ParseSingle] ex: {ex.Message}");
        }
        return results;
    }

    private List<LoggedWorkoutRecord> ParseWorkoutRecords(string body, string? userId)
    {
        var results = new List<LoggedWorkoutRecord>();
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
            {
                System.Diagnostics.Debug.WriteLine("[PB MyWorkouts] no 'items' array in response");
                return results;
            }

            foreach (var item in items.EnumerateArray())
            {
                var record = ParseWorkoutItem(item);
                if (record != null && (userId == null || record.User == userId))
                    results.Add(record);
            }

            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] parsed {results.Count} items (userId='{userId}') from {items.GetArrayLength()} total");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB MyWorkouts] ParseWorkoutRecords ex: {ex}");
        }
        return results;
    }

    private static LoggedWorkoutRecord? ParseWorkoutItem(JsonElement item)
    {
        try
        {
            var record = new LoggedWorkoutRecord();
            if (item.TryGetProperty("id", out var idEl)) record.Id = idEl.GetString() ?? "";
            if (item.TryGetProperty("name", out var nameEl)) record.Name = nameEl.GetString() ?? "";
            if (item.TryGetProperty("date", out var dateEl)) record.Date = dateEl.GetString() ?? "";
            if (item.TryGetProperty("notes", out var notesEl)) record.Notes = notesEl.GetString() ?? "";
            if (item.TryGetProperty("exercise_data", out var exDataEl))
            {
                if (exDataEl.ValueKind == JsonValueKind.String)
                    record.ExerciseData = exDataEl.GetString() ?? "";
                else
                    record.ExerciseData = exDataEl.GetRawText();
            }
            if (item.TryGetProperty("user_name", out var unameEl)) record.UserName = unameEl.GetString() ?? "";
            if (item.TryGetProperty("volume", out var volEl) && volEl.TryGetDouble(out var v)) record.Volume = v;
            if (item.TryGetProperty("duration", out var durEl) && durEl.TryGetInt32(out var d)) record.Duration = d;
            if (item.TryGetProperty("likes", out var likesEl) && likesEl.TryGetInt32(out var l)) record.Likes = l;

            if (item.TryGetProperty("user", out var userEl))
            {
                if (userEl.ValueKind == JsonValueKind.String)
                    record.User = userEl.GetString() ?? "";
                else if (userEl.ValueKind == JsonValueKind.Object && userEl.TryGetProperty("id", out var uidEl))
                    record.User = uidEl.GetString() ?? "";
            }

            if (item.TryGetProperty("exercises", out var exArr) && exArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var ex in exArr.EnumerateArray())
                    record.Exercises.Add(ex.GetString() ?? "");
            }

            if (item.TryGetProperty("liked_by", out var likedArr) && likedArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var lb in likedArr.EnumerateArray())
                    record.LikedBy.Add(lb.GetString() ?? "");
            }

            if (item.TryGetProperty("photos", out var photosArr) && photosArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in photosArr.EnumerateArray())
                    record.Photos.Add(p.GetString() ?? "");
            }

            return record;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB ParseItem] ex: {ex.Message}");
            return null;
        }
    }

    public async Task<List<LoggedWorkoutRecord>> GetFollowedWorkoutsAsync()
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return new();
        try
        {
            var followingIds = await GetFollowingUserIdsAsync();
            System.Diagnostics.Debug.WriteLine($"[PB FollowedWorkouts] followingIds={string.Join(",", followingIds)}");
            if (followingIds.Count == 0) return new();

            var url = $"collections/logged_workouts/records?perPage=30";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await TryHandle401Async(response))
                {
                    request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    response = await GetHttp().SendAsync(request);
                }
            }
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[PB FollowedWorkouts] FAIL status={response.StatusCode} body={body[..Math.Min(body.Length, 200)]}");
#endif
                return new();
            }
            var records = ParseWorkoutRecords(body, null);
            records = records.Where(w => followingIds.Contains(w.User)).ToList();

            foreach (var r in records)
            {
                if (!string.IsNullOrWhiteSpace(r.User))
                {
                    try
                    {
                        var userReq = new HttpRequestMessage(HttpMethod.Get, $"collections/users/records/{r.User}");
                        userReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var userRes = await GetHttp().SendAsync(userReq);
                        if (userRes.IsSuccessStatusCode)
                        {
                            var userRecord = await userRes.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
                            if (userRecord != null)
                            {
                                r.UserName = userRecord.Name;
                                if (!string.IsNullOrWhiteSpace(userRecord.Avatar))
                                    r.AvatarUrl = GetFileUrl(userRecord.CollectionId, userRecord.Id, userRecord.Avatar);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PB FollowedWk] userRes err: {ex.Message}");
                    }
                }
            }
            return records;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB FollowedWk] ex: {ex.Message}");
            return new();
        }
    }

    public async Task<(bool Success, string Error)> LikeWorkoutAsync(string workoutId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return (false, "Non autenticato.");

        try
        {
            var getReq = new HttpRequestMessage(HttpMethod.Get, $"collections/logged_workouts/records/{workoutId}");
            getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var getRes = await GetHttp().SendAsync(getReq);
            if (!getRes.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"[PB Like] GET fail {getRes.StatusCode}");
                return (false, "Impossibile leggere il workout.");
            }

            var body = await getRes.Content.ReadAsStringAsync();
            var (likedBy, likes) = ParseLikesFromResponse(body);
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
            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                var errBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[PB Like] PATCH fail {response.StatusCode}: {errBody[..Math.Min(errBody.Length, 50)]}");
#endif
                return (false, "Like non riuscito. Verifica API Rule 'Update' su PocketBase.");
            }
            System.Diagnostics.Debug.WriteLine($"[PB Like] OK, likes={likedBy.Count}");
            return (true, string.Empty);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server."); }
    }

    public async Task<(bool Success, string Error)> UnlikeWorkoutAsync(string workoutId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return (false, "Non autenticato.");

        try
        {
            var getReq = new HttpRequestMessage(HttpMethod.Get, $"collections/logged_workouts/records/{workoutId}");
            getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var getRes = await GetHttp().SendAsync(getReq);
            if (!getRes.IsSuccessStatusCode) return (false, "Workout non trovato.");

            var body = await getRes.Content.ReadAsStringAsync();
            var (likedBy, likes) = ParseLikesFromResponse(body);
            likedBy.Remove(currentUser.Id);
            var payload = new { liked_by = likedBy, likes = likedBy.Count };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/logged_workouts/records/{workoutId}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            return response.IsSuccessStatusCode ? (true, string.Empty) : (false, "Errore unlike.");
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[PB] ex: {ex.Message}"); return (false, "Errore del server."); }
    }

    private static (List<string> LikedBy, int Likes) ParseLikesFromResponse(string body)
    {
        var likedBy = new List<string>();
        var likes = 0;
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("liked_by", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in arr.EnumerateArray())
                    likedBy.Add(el.GetString() ?? "");
            }
            if (doc.RootElement.TryGetProperty("likes", out var l) && l.TryGetInt32(out var lv))
                likes = lv;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB ParseLikes] ex: {ex.Message}");
        }
        return (likedBy, likes);
    }

    public async Task<List<(string LikerName, string WorkoutName, string WorkoutId)>> GetLikeNotificationsAsync()
    {
        var results = new List<(string, string, string)>();
        await EnsureAuthAsync();
        if (!IsLoggedIn || currentUser == null) return results;

        try
        {
            var workouts = await GetMyWorkoutsAsync(50);
            foreach (var w in workouts)
            {
                var externalLikers = w.LikedBy.Where(id => id != currentUser.Id).ToList();
                foreach (var likerId in externalLikers)
                {
                    try
                    {
                        var userReq = new HttpRequestMessage(HttpMethod.Get,
                            $"collections/users/records/{likerId}");
                        userReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var userRes = await GetHttp().SendAsync(userReq);
                        if (userRes.IsSuccessStatusCode)
                        {
                            var userRecord = await userRes.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
                            var likerName = userRecord?.Name ?? "Unknown";
                            results.Add((likerName, w.Name, w.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PB LikeNotif] userRes err: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB LikeNotif] ex: {ex.Message}");
        }
        return results;
    }

    public async Task<string?> GetCachedExerciseImageAsync(string exerciseName)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return null;
        try
        {
            var filter = $"name=\"{exerciseName.Replace("\"", "\\\"")}\"";
            var url = $"collections/excercise/records?filter={Uri.EscapeDataString(filter)}&perPage=1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<CachedExerciseListResponse>(body, JsonOptions);
            var item = list?.Items?.FirstOrDefault();
            return item?.ImageUrl;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB CacheImg] ex: {ex.Message}");
            return null;
        }
    }

    public class CachedExerciseListResponse
    {
        public List<CachedExerciseItem> Items { get; set; } = new();
    }

    public class CachedExerciseItem
    {
        public string ImageUrl { get; set; } = "";
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PB ParseErr] ex: {ex.Message}");
        }

        return "Errore dal server. Riprova più tardi.";
    }
}
