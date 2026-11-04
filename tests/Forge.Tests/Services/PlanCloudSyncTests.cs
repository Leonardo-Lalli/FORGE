using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Forge.Services;

namespace Forge.Tests.Services;

[Collection("Account")]
public sealed class PlanCloudSyncTests : IAsyncLifetime
{
    private readonly List<string> files = [];
    private readonly Cloud cloud = new();
    private readonly JsonSerializerOptions json = new(JsonSerializerDefaults.Web);
    private DatabaseService Database()
    {
        var path = Path.Combine(Path.GetTempPath(), $"forge-cloud-{Guid.NewGuid():N}.db");
        files.Add(path);
        return new(path);
    }
    private async Task<PocketBaseService> Login(string user)
    {
        Preferences.Set("pb_server_url", "https://example.invalid");
        var pb = new PocketBaseService(cloud, new BuildSecrets());
        Assert.True((await pb.LoginAsync(user, "password")).Success);
        return pb;
    }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync()
    {
        foreach (var path in files)
        {
            await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync();
            File.Delete(path);
        }
    }
    [Fact]
    public async Task FreshInstallRestoresOwnPlansWithSetsAndNotes()
    {
        var pb = await Login("alice");
        var original = new WorkoutPlan { Name = "My routine", Notes = "Line 1\nLine 2", RestSeconds = 125,
            Exercises = [new() { ExerciseName = "Squat", Notes = "Slow", Sets = [new() { WeightKg = 60, Reps = 8 }] }] };
        var service = new PlanService(Database(), pb);
        await service.SavePlanAsync(original);
        Assert.Empty(service.SyncError);
        var reinstalled = new PlanService(Database(), pb);
        var restored = Assert.Single(await reinstalled.LoadPlansAsync());
        Assert.Equal(original.Id, restored.Id); Assert.Equal(original.Notes, restored.Notes);
        Assert.Equal(125, restored.RestSeconds); Assert.Equal(60, restored.Exercises[0].Sets[0].WeightKg);
        var bob = new PlanService(Database(), await Login("bob"));
        Assert.Empty(await bob.LoadPlansAsync());
    }
    [Fact]
    public async Task OfflineEditsAndDeletesRetryWithoutResurrectingDeletedPlans()
    {
        var pb = await Login("alice");
        var db = Database(); var service = new PlanService(db, pb);
        var plan = new WorkoutPlan { Name = "Offline routine" };
        await service.SavePlanAsync(plan);
        var other = new PlanService(Database(), pb);
        await other.LoadPlansAsync();
        cloud.Offline = true;
        plan.Notes = "Offline note"; await service.SavePlanAsync(plan);
        Assert.True(service.HasSyncError); Assert.True(Assert.Single(await db.GetPlansAsync("alice")).PendingSync);
        Assert.Equal("Offline note", Assert.Single(await service.LoadPlansAsync()).Notes);
        await service.DeletePlanAsync(plan.Id);
        Assert.Empty(await service.LoadPlansAsync());
        cloud.Offline = false;
        Assert.True(await service.SyncPlansAsync());
        Assert.Empty(await other.LoadPlansAsync());
        Assert.True(Assert.Single(await db.GetPlansAsync("alice")).IsDeleted);
        Assert.False(Assert.Single(await db.GetPlansAsync("alice")).PendingSync);
    }
    [Fact]
    public async Task FailedSyncDoesNotCreateExamplesThatCouldHideServerPlans()
    {
        var pb = await Login("new-owner");
        cloud.Offline = true;
        var service = new PlanService(Database(), pb);
        await service.EnsureStarterPlansAsync();
        Assert.True(service.HasSyncError);
        Assert.Empty(await service.LoadPlansAsync());
    }
    private sealed class Cloud : HttpMessageHandler, IHttpClientFactory
    {
        private readonly Dictionary<string, SavedPlan> plans = [];
        private readonly JsonSerializerOptions json = new(JsonSerializerDefaults.Web);
        public bool Offline { get; set; }
        public HttpClient CreateClient(string name) => new(this, false);
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var body = await request.Content!.ReadAsStringAsync(token);
            if (request.RequestUri!.AbsolutePath.EndsWith("auth-with-password"))
            {
                var owner = JsonDocument.Parse(body).RootElement.GetProperty("identity").GetString()!;
                return new(HttpStatusCode.OK) { Content = JsonContent.Create(new { token = owner, record = new { id = owner } }) };
            }
            if (Offline) return new(HttpStatusCode.ServiceUnavailable);
            var ownerId = request.Headers.Authorization!.Parameter!;
            var changes = JsonSerializer.Deserialize<List<SavedPlan>>(JsonDocument.Parse(body).RootElement.GetProperty("plans").GetRawText(), json)!;
            foreach (var plan in changes)
            {
                Assert.Equal(ownerId, plan.UserId);
                var key = ownerId + ":" + plan.Id;
                if (!plans.TryGetValue(key, out var saved) || saved.UpdatedAt < plan.UpdatedAt) plans[key] = plan;
            }
            return new(HttpStatusCode.OK) { Content = JsonContent.Create(new { plans = plans.Values.Where(p => p.UserId == ownerId).ToList() }) };
        }
    }
}
