using System.Net;
using System.Text;
using System.Text.Json;
using Forge.Services;
namespace Forge.Tests;

[Collection("Account")]
public class ProfileWorkoutRegressionTests : IAsyncLifetime
{
    private readonly string path = Path.Combine(Path.GetTempPath(), $"forge-features-{Guid.NewGuid():N}.db");
    private DatabaseService db = null!;
    public Task InitializeAsync() { db = new(path); return Task.CompletedTask; }
    public async Task DisposeAsync()
    { await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync(); File.Delete(path); }
    [Fact]
    public async Task PlansAreOwnedAndDeletedExamplesNeverRegenerate()
    {
        var pb = new PocketBaseService(new Factory(req => Json("{\"token\":\"t\",\"record\":{\"id\":\"" + (req.Content!.ReadAsStringAsync().Result.Contains("bob") ? "bob" : "alice") + "\"}}")), new BuildSecrets());
        Preferences.Set("pb_server_url", "https://example.invalid");
        Preferences.Remove("starter_plans_v1:alice"); Preferences.Remove("starter_plans_v1:bob");
        await pb.LoginAsync("alice@test.invalid", "password");
        var service = new PlanService(db, pb);
        await service.EnsureStarterPlansAsync();
        Assert.Equal(2, (await service.LoadPlansAsync()).Count);
        foreach (var plan in await service.LoadPlansAsync()) await service.DeletePlanAsync(plan.Id);
        await service.EnsureStarterPlansAsync(); Assert.Empty(await service.LoadPlansAsync());
        var own = new WorkoutPlan { Name = "Own", Notes = "Plan notes", Exercises = [new WorkoutExercise { Notes = "Exercise notes", RestSeconds = 125 }] };
        await service.SavePlanAsync(own);
        for (var i = 0; i < 5; i++) await service.SavePlanAsync(new() { Name = "Unlimited " + i });
        Assert.Equal(6, (await service.LoadPlansAsync()).Count);
        var restored = (await service.LoadPlansAsync()).Single(p => p.Id == own.Id);
        Assert.Equal("Plan notes", restored.Notes); Assert.Equal(125, restored.Exercises[0].RestSeconds); Assert.Equal("Exercise notes", restored.Exercises[0].Notes);
        await pb.LoginAsync("bob@test.invalid", "password");
        Assert.Empty(await service.LoadPlansAsync());
        await service.DeletePlanAsync(own.Id);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SavePlanAsync(own));
        await service.EnsureStarterPlansAsync(); Assert.Equal(2, (await service.LoadPlansAsync()).Count);
        await pb.LoginAsync("alice@test.invalid", "password"); Assert.Equal(6, (await service.LoadPlansAsync()).Count);
        pb.Logout();
    }
    [Fact]
    public async Task RetroactiveAwardsImportEveryHistoryPage()
    {
        var pb = new PocketBaseService(new Factory(req => {
            if (req.RequestUri!.AbsolutePath.EndsWith("auth-with-password")) return Json("{\"token\":\"t\",\"record\":{\"id\":\"alice\"}}");
            var second = req.RequestUri.Query.Contains("page=2&");
            var rows = Enumerable.Range(second ? 200 : 0, second ? 1 : 200).Select(i => new { id = "w" + i, user = "alice", date = DateTime.Today.AddDays(-i).ToString("O"), duration = 10, volume = 100 });
            return Json(JsonSerializer.Serialize(new { page = second ? 2 : 1, totalPages = 2, items = rows }));
        }), new BuildSecrets());
        Preferences.Set("pb_server_url", "https://example.invalid"); await pb.LoginAsync("a@test.invalid", "password");
        var service = new AchievementService(db, pb);
        var all = await service.GetAllAsync();
        Assert.True(all.Single(x => x.Def.Id == "first_workout").State.IsUnlocked);
        Assert.True(all.Single(x => x.Def.Id == "member").State.IsUnlocked);
        Assert.Equal(201, (await db.GetWorkoutsAsync("alice")).Count); pb.Logout();
    }
    [Fact]
    public void ChartDoesNotMultiplySameDayVolume()
    {
        var today = DateTime.Today;
        var result = VolumeChart.Build([new() { Date = today.ToString("O"), Volume = 100 }, new() { Date = today.ToString("O"), Volume = 200 }], "week", today);
        Assert.Equal(300, result.Sum(b => b.Volume)); Assert.Equal(140, result.Last().Height);
        Assert.Equal(result.Count, result.Select(b => b.Label).Distinct().Count());
        Assert.All(VolumeChart.Build([new() { Date = today.ToString("O"), Volume = 0 }], "week", today), b => Assert.Equal(0, b.Height));
    }
    [Fact]
    public void GhostSetsUseLastCompletedSessionAndLoseSuggestionAfterEditing()
    {
        var exercise = new WorkoutExercise { ExerciseId = "squat", ExerciseName = "Squat", Sets = [new() { SetNumber = 1 }] };
        var history = new List<LocalWorkout> {
            new() { Date = "2026-10-01", ExerciseDataJson = "[{\"exerciseId\":\"squat\",\"name\":\"Squat\",\"sets\":[{\"weightKg\":80,\"reps\":8,\"isCompleted\":true}]}]" },
            new() { IsDraft = true, Date = "2026-10-02", ExerciseDataJson = "[{\"name\":\"Squat\",\"sets\":[{\"weightKg\":100,\"reps\":1}]}]" }
        };
        PreviousPerformance.Apply(history, exercise);
        Assert.Equal(80, exercise.Sets[0].WeightKg); Assert.Equal(8, exercise.Sets[0].Reps); Assert.True(exercise.Sets[0].IsSuggested);
        exercise.Sets[0].Reps = 9; Assert.False(exercise.Sets[0].IsSuggested);
    }
    [Fact]
    public void RestPickersSupportZeroAndFiveMinutesWithoutOverflow()
    {
        var e = new WorkoutExercise { RestSeconds = 125 }; Assert.Equal(2, e.RestMinutes); Assert.Equal(5, e.RestSecondPart);
        e.RestMinutes = 5; Assert.Equal(300, e.RestSeconds); Assert.Equal(0, e.RestSecondPart);
        e.RestSeconds = 0; Assert.Equal(0, e.RestMinutes); Assert.Equal(0, e.RestSecondPart);
        e.RestSeconds = 700; Assert.Equal(300, e.RestSeconds);
    }
    [Fact]
    public async Task ProtectedAvatarRetriesWithFileTokenInsteadOfAuthenticationTokenInUrl()
    {
        var tokenRequested = false;
        var pb = new PocketBaseService(new Factory(req => {
            if (req.RequestUri!.AbsolutePath.EndsWith("auth-with-password")) return Json("{\"token\":\"auth-secret\",\"record\":{\"id\":\"alice\"}}");
            if (req.RequestUri.AbsolutePath.EndsWith("files/token")) { tokenRequested = true; return Json("{\"token\":\"short-file-token\"}"); }
            Assert.DoesNotContain("auth-secret", req.RequestUri.ToString());
            return req.RequestUri.Query.Contains("short-file-token") ? new(HttpStatusCode.OK) { Content = new ByteArrayContent([1, 2, 3]) } : new(HttpStatusCode.NotFound);
        }), new BuildSecrets());
        Preferences.Set("pb_server_url", "https://example.invalid");
        await pb.LoginAsync("alice@test.invalid", "password");
        Assert.NotNull(await pb.DownloadAvatarAsync("users", "alice", "photo.png"));
        Assert.True(tokenRequested);
        pb.Logout();
    }
    [Fact]
    public void MissingJsonBadgeSelectionIsAnEmptyList()
    {
        Assert.Empty(JsonSerializer.Deserialize<PocketBaseUserRecord>("{\"featured_achievements\":null}")!.FeaturedAchievements);
        var record = JsonSerializer.Deserialize<LoggedWorkoutRecord>("{\"exercises\":null,\"liked_by\":null,\"photos\":null}")!;
        Assert.Empty(record.Exercises); Assert.Empty(record.LikedBy); Assert.Empty(record.Photos);
    }
    [Fact]
    public void WorkoutJsonAcceptsArrayOrString()
    {
        Assert.Equal("[]", JsonSerializer.Deserialize<LoggedWorkoutRecord>("{\"exercise_data\":[]}")!.ExerciseData);
        Assert.Equal("[]", JsonSerializer.Deserialize<LoggedWorkoutRecord>("{\"exercise_data\":\"[]\"}")!.ExerciseData);
    }
    private static HttpResponseMessage Json(string value) => new(HttpStatusCode.OK) { Content = new StringContent(value, Encoding.UTF8, "application/json") };
    private sealed class Factory(Func<HttpRequestMessage, HttpResponseMessage> respond) : IHttpClientFactory { public HttpClient CreateClient(string name) => new(new Handler(respond)); }
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler { protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(respond(request)); }
}
