using System.Net.Http.Headers;
using System.Net.Http.Json;
using Forge.Models.Dto;

namespace Forge.Services;
public partial class PocketBaseService
{
    public async Task<string?> GetCatalogPageAsync(string cursor = "", bool status = false)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return null;
        using var request = new HttpRequestMessage(HttpMethod.Get, status ? "forge/catalog/status" :
            "forge/catalog/page" + (string.IsNullOrEmpty(cursor) ? "" : "?after=" + Uri.EscapeDataString(cursor)));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            using var response = await GetHttp().SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { return null; }
    }
    public async Task<List<string>> GetWorkoutPhotoUrlsAsync(LoggedWorkoutRecord workout)
    {
        if (workout.Photos.Count == 0) return [];
        // JSON URL fields are retained; PocketBase file fields contain bare filenames.
        string? fileToken = null;
        if (workout.Photos.Any(p => !Uri.TryCreate(p, UriKind.Absolute, out _)))
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "files/token") { Content = JsonContent.Create(new { }) };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await GetHttp().SendAsync(request);
            response.EnsureSuccessStatusCode();
            using var body = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            fileToken = body.RootElement.GetProperty("token").GetString();
        }
        return workout.Photos.Select(p => Uri.TryCreate(p, UriKind.Absolute, out _) ? p :
            GetFileUrl("logged_workouts", workout.Id, p) + "?token=" + Uri.EscapeDataString(fileToken ?? "")).ToList();
    }
    private static string FilterValue(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    private async Task<List<T>> ReadAllAsync<T>(string collection, string filter)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return [];
        var result = new List<T>();
        var retriedPage = 0;
        for (var page = 1; ; page++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"collections/{collection}/records?page={page}&perPage=200&filter={Uri.EscapeDataString(filter)}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await GetHttp().SendAsync(request);
            if (retriedPage != page && await TryHandle401Async(response)) { retriedPage = page; page--; continue; }
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<T>>(JsonOptions)
                ?? throw new System.Text.Json.JsonException("Risposta elenco non valida.");
            result.AddRange(data.Items);
            if (data.Items.Count == 0 || page >= data.TotalPages) break;
        }
        return result;
    }
    public async Task<List<LoggedWorkoutRecord>> GetUserWorkoutsAsync(string userId, int limit = 0) =>
        (await ReadAllAsync<LoggedWorkoutRecord>("logged_workouts", "user = " + FilterValue(userId)))
        .Where(w => w.User == userId).OrderByDescending(w => w.Date).Take(limit > 0 ? limit : int.MaxValue).ToList();
    public async Task<PocketBaseUserRecord?> GetUserByIdAsync(string userId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return null;
        using var request = new HttpRequestMessage(HttpMethod.Get, "collections/users/records/" + Uri.EscapeDataString(userId));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await GetHttp().SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
    }
    public async Task<(bool Allowed, List<SocialGraphRecord> Items)> GetVisibleConnectionsAsync(string userId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return (false, []);
        using var request = new HttpRequestMessage(HttpMethod.Get, "forge/connections/" + Uri.EscapeDataString(userId));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await GetHttp().SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return userId == currentUser?.Id ? (true, (await GetOwnRelationshipsAsync()).Where(r => r.Status == "accepted").ToList()) : (false, []);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<ConnectionsResponse>(JsonOptions);
        return (data?.Allowed == true, data?.Items ?? []);
    }
    private sealed class ConnectionsResponse
    {
        public bool Allowed { get; set; }
        public List<SocialGraphRecord> Items { get; set; } = [];
    }
    public async Task<List<SocialGraphRecord>> GetConnectionsAsync(string userId) => (await GetVisibleConnectionsAsync(userId)).Items;
    public async Task<string> GetLatestPublicWorkoutIdAsync(string userId)
    {
        await EnsureAuthAsync();
        if (!IsLoggedIn) return "";
        var filter = "user = " + FilterValue(userId) + " && user.workout_visibility = 'public'";
        using var request = new HttpRequestMessage(HttpMethod.Get, "collections/logged_workouts/records?perPage=1&sort=-date&filter=" + Uri.EscapeDataString(filter));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await GetHttp().SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PocketBaseListResponse<LoggedWorkoutRecord>>(JsonOptions);
        return result?.Items.FirstOrDefault()?.Id ?? "";
    }
    public async Task<bool> UnfollowUserAsync(string userId)
    {
        var outgoing = (await GetOwnRelationshipsAsync()).Where(r => r.FromUser == currentUser?.Id && r.ToUser == userId && r.Status is "accepted" or "pending").ToList();
        foreach (var relation in outgoing)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, "collections/social_graph/records/" + Uri.EscapeDataString(relation.Id));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await GetHttp().SendAsync(request);
            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound) return false;
        }
        return true;
    }
    public Task<List<SocialGraphRecord>> GetOwnRelationshipsAsync() =>
        ReadAllAsync<SocialGraphRecord>("social_graph", $"from_user = {FilterValue(currentUser?.Id ?? "")} || to_user = {FilterValue(currentUser?.Id ?? "")}");
    public async Task<(bool Success, string Error)> UpdateProfileOptionsAsync(string name, string bio, string visibility, List<string> badges, string? connectionsVisibility = null)
    {
        if (currentUser == null) return (false, "Non autenticato.");
        if (!new[] { "public", "private", "friends" }.Contains(visibility) || badges.Count > 3 || badges.Distinct().Count() != badges.Count)
            return (false, "Opzioni profilo non valide.");
        if (connectionsVisibility != null && !new[] { "public", "private", "friends" }.Contains(connectionsVisibility))
            return (false, "Privacy follower non valida.");
        var payload = new Dictionary<string, object> { ["name"] = name, ["bio"] = bio, ["workout_visibility"] = visibility, ["featured_achievements"] = badges };
        if (connectionsVisibility != null) payload["connections_visibility"] = connectionsVisibility;
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"collections/users/records/{currentUser.Id}")
        {
            Content = JsonContent.Create(payload, options: JsonOptions)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await GetHttp().SendAsync(request);
        if (!response.IsSuccessStatusCode) return (false, ParseErrorMessage(await response.Content.ReadAsStringAsync()));
        var user = await response.Content.ReadFromJsonAsync<PocketBaseUserRecord>(JsonOptions);
        if (user == null || user.WorkoutVisibility != visibility || !user.FeaturedAchievements.SequenceEqual(badges))
            return (false, "Il server non supporta ancora privacy e badge: applica la migrazione profili. Le opzioni non sono state confermate.");
        if (connectionsVisibility != null && user.ConnectionsVisibility != connectionsVisibility)
            return (false, "Applica sul server la nuova migrazione privacy follower e seguiti.");
        currentUser = user;
        return (true, "");
    }
}
