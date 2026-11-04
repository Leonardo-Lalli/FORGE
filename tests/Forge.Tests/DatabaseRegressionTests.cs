using Forge.Services;

namespace Forge.Tests;

public sealed class DatabaseRegressionTests : IAsyncLifetime
{
    private readonly string path = Path.Combine(Path.GetTempPath(), $"forge-audit-{Guid.NewGuid():N}.db");
    private DatabaseService db = null!;
    public Task InitializeAsync() { db = new(path); return Task.CompletedTask; }
    public async Task DisposeAsync()
    {
        await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync();
        File.Delete(path);
    }

    [Fact]
    public async Task ConcurrentFirstUseWaitsForAllTables()
    {
        await Task.WhenAll(Enumerable.Range(0, 40).Select(i => db.SaveWorkoutAsync(new LocalWorkout { Id = i.ToString(), UserId = "alice" })));
        Assert.Equal(40, (await db.GetWorkoutsAsync("alice")).Count);
    }

    [Fact]
    public async Task PendingQueueExcludesDraftsAndAccountReadsAreIsolated()
    {
        await db.SaveWorkoutAsync(new() { Id = "draft", UserId = "alice", IsDraft = true, PendingSync = true });
        await db.SaveWorkoutAsync(new() { Id = "done", UserId = "bob", PendingSync = true });
        Assert.Equal("done", Assert.Single(await db.GetPendingSyncAsync()).Id);
        Assert.Equal("draft", Assert.Single(await db.GetWorkoutsAsync("alice")).Id);
    }

    [Fact]
    public async Task SuccessfulSyncReconcilesLocalIdWithoutDuplicateAndPreservesPhotos()
    {
        await db.SaveWorkoutAsync(new() { Id = "legacy-guid", PhotosJson = "[\"photo\"]", PendingSync = true });
        await db.MarkSyncedAsync("legacy-guid", "abcdefghijk1234");
        var saved = Assert.Single(await db.GetWorkoutsAsync());
        Assert.Equal("abcdefghijk1234", saved.Id);
        Assert.Equal("[\"photo\"]", saved.PhotosJson);
        Assert.False(saved.PendingSync);
        Assert.Empty(await db.GetPendingSyncAsync());
    }

    [Fact]
    public async Task CompletingWorkoutRemovesOnlyItsDraftInTheSameTransaction()
    {
        await db.SaveWorkoutAsync(new() { Id = "draft", UserId = "alice", IsDraft = true });
        await db.SaveWorkoutAsync(new() { Id = "other", UserId = "bob", IsDraft = true });
        await db.CompleteWorkoutAsync(new() { Id = "finished", UserId = "alice", PendingSync = true }, "draft");
        Assert.False(Assert.Single(await db.GetWorkoutsAsync("alice")).IsDraft);
        Assert.True(Assert.Single(await db.GetWorkoutsAsync("bob")).IsDraft);
        Assert.Equal("finished", Assert.Single(await db.GetPendingSyncAsync()).Id);
    }

    [Fact]
    public async Task UpgradeAddsPlanSyncColumnsWithoutLosingExistingPlans()
    {
        var legacy = new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache);
        await legacy.CreateTableAsync<LegacyPlan>();
        await legacy.InsertAsync(new LegacyPlan { Id = "existing", UserId = "alice", Name = "Before update", ExercisesJson = "[]", Notes = "Keep my notes" });
        var saved = Assert.Single(await db.GetPlansAsync("alice"));
        Assert.Equal("Before update", saved.Name);
        Assert.Equal("Keep my notes", saved.Notes);
        Assert.False(saved.IsDeleted);
        saved.PendingSync = true;
        await db.SavePlanAsync(saved);
        Assert.Equal("existing", Assert.Single(await new DatabaseService(path).GetPlansAsync("alice")).Id);
    }

    [SQLite.Table("saved_plans")]
    public class LegacyPlan
    {
        [SQLite.PrimaryKey] public string Id { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Notes { get; set; } = "";
        public string ExercisesJson { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int RestSeconds { get; set; } = 90;
    }

    [Fact]
    public async Task NativeSqliteIsAtLeastTheSecurityFixedVersion()
    {
        SQLitePCL.Batteries_V2.Init();
        var connection = new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache);
        var version = await connection.ExecuteScalarAsync<string>("select sqlite_version()");
        Assert.True(Version.Parse(version) >= new Version(3, 50, 2), version);
    }
}



