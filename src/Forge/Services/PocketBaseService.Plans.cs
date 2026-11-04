using System.Net.Http.Json;
using Forge.Models;

namespace Forge.Services;

public partial class PocketBaseService
{
    public async Task<List<SavedPlan>> SyncPlansAsync(string ownerId, List<SavedPlan> changes)
    {
        await EnsureAuthAsync();
        if (CurrentUser?.Id != ownerId || !IsLoggedIn)
            throw new InvalidOperationException("L’account è cambiato durante la sincronizzazione.");
        using var request = new HttpRequestMessage(HttpMethod.Post, "forge/plans/sync")
        { Content = JsonContent.Create(new { plans = changes }) };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        using var response = await GetHttp().SendAsync(request, timeout.Token);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PlanSyncResponse>(timeout.Token)
            ?? throw new System.Text.Json.JsonException("Risposta schede non valida.");
        if (result.Plans == null) throw new System.Text.Json.JsonException("Elenco schede non valido.");
        if (CurrentUser?.Id != ownerId || result.Plans.Any(p => p.UserId != ownerId))
            throw new InvalidOperationException("Risposta appartenente a un altro account.");
        if (changes.Any(change => !result.Plans.Any(p => p.Id == change.Id)))
            throw new InvalidOperationException("Il server non ha confermato tutte le schede.");
        return result.Plans;
    }

    private sealed class PlanSyncResponse
    {
        [System.Text.Json.Serialization.JsonRequired]
        public List<SavedPlan> Plans { get; set; } = [];
    }
}
