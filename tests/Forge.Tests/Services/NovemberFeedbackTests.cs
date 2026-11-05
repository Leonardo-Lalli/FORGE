using System.Text;
using System.Text.Json;
using Forge.Services;

namespace Forge.Tests.Services;

public class NovemberFeedbackTests
{
    private static WorkoutPlan Plan() => new() { Name = "Upper", Exercises = [new() { ExerciseName = "Curl", ExerciseId = "curl", Sets = [new() { SetNumber = 1, Reps = 10, WeightKg = 20 }, new() { SetNumber = 2, Reps = 8, WeightKg = 22 }] }] };
    private static PocketBaseService Backend() => new(new ClientFactory(), new BuildSecrets());
    private sealed class ClientFactory : IHttpClientFactory { public HttpClient CreateClient(string name) => new(); }

    [Fact]
    public void RemovingCompletedOrLastSetRenumbersAndExcludesEmptyExercise()
    {
        var exercise = Plan().Exercises[0];
        exercise.Sets[0].IsCompleted = true;
        WorkoutSetEditor.Remove(exercise, exercise.Sets[0]);
        Assert.Equal(1, Assert.Single(exercise.Sets).SetNumber);
        Assert.Empty(WorkoutCompletion.LoggedExercises([exercise]));
        WorkoutSetEditor.Remove(exercise, exercise.Sets[0]);
        Assert.Empty(exercise.Sets);
        Assert.False(exercise.IsCompleted);
    }

    [Theory]
    [InlineData("forearms", true)]
    [InlineData("brachioradialis", true)]
    [InlineData("wrist flexors", true)]
    [InlineData("biceps", false)]
    public void ForearmFilterUsesPrimaryTargets(string primary, bool expected)
        => Assert.Equal(expected, ExerciseMuscleFilter.Matches("lower arms", primary, "", ExerciseMuscleFilter.ForearmAliases.Split(',')));

    [Fact]
    public void RenameWithoutCompletedSetsDescribesOnlyName()
    {
        var original = Plan();
        var current = PlanSessionChanges.CopyExercises(original.Exercises);
        Assert.All(current.SelectMany(e => e.Sets), s => Assert.False(s.IsCompleted));
        var text = Assert.Single(PlanSessionChanges.Describe(original, "Upper B", 90, current));
        Assert.StartsWith("Nome della scheda modificato", text);
        Assert.Contains("Upper B", text);
    }

    [Fact]
    public void RenameAddedSetAndReplacementProduceExactlyThoseChanges()
    {
        var original = Plan();
        var current = PlanSessionChanges.CopyExercises(original.Exercises);
        current[0].ExerciseId = "hammer"; current[0].ExerciseName = "Hammer curl";
        current[0].Sets.Add(new() { SetNumber = 3, Reps = 8 });
        var descriptions = PlanSessionChanges.Describe(original, "Upper B", 90, current);
        Assert.Equal(3, descriptions.Count);
        Assert.Contains(descriptions, x => x.StartsWith("Nome"));
        Assert.Contains(descriptions, x => x.StartsWith("Serie aggiunta"));
        Assert.Contains(descriptions, x => x.StartsWith("Esercizio sostituito"));
    }

    [Fact]
    public void CompletionNotesAndSuggestionsDoNotAppearAsStructuralChanges()
    {
        var original = Plan();
        var current = PlanSessionChanges.CopyExercises(original.Exercises);
        current[0].Sets[0].IsCompleted = true; current[0].Sets[0].IsSuggested = true; current[0].Notes = "New note";
        Assert.Empty(PlanSessionChanges.Describe(original, original.Name, 90, current));
        current[0].Sets[0].WeightKg += 2;
        Assert.StartsWith("Carichi", Assert.Single(PlanSessionChanges.Describe(original, original.Name, 90, current)));
    }

    [Fact]
    public void RemovingFirstAndAddingDifferentLastSetKeepsChangeAttribution()
    {
        var original = Plan(); var current = PlanSessionChanges.CopyExercises(original.Exercises);
        current[0].Sets.RemoveAt(0); current[0].Sets.Add(new() { Reps = 6, WeightKg = 30 });
        var changes = PlanSessionChanges.Describe(original, original.Name, 90, current);
        Assert.Equal(2, changes.Count);
        Assert.Contains(changes, c => c.StartsWith("Serie aggiunta"));
        Assert.Contains(changes, c => c.StartsWith("Serie rimossa"));
    }

    [Theory]
    [InlineData(2026, 10, 31)]
    [InlineData(2024, 2, 29)]
    [InlineData(2025, 12, 31)]
    public void SelectedMonthHasBothBoundsAndFullChart(int year, int month, int days)
    {
        var selected = new DateTime(year, month, 1);
        var period = StatsPeriod.Range("month", new(2026, 11, 5), selectedMonth: selected);
        Assert.Equal(selected, period.Start); Assert.Equal(selected.AddMonths(1), period.EndExclusive);
        var source = new List<LoggedWorkoutRecord> {
            new() { Date = selected.AddDays(2).AddHours(12).ToString("O"), Volume = 100 },
            new() { Date = selected.AddMonths(1).AddHours(12).ToString("O"), Volume = 999 }
        };
        var filtered = source.Where(w => AchievementEvaluator.ParseDate(w.Date) is { } d && d >= period.Start && d < period.EndExclusive).ToList();
        var chart = VolumeChart.Build(filtered, "month", period.EndExclusive.AddDays(-1));
        Assert.Single(filtered); Assert.Equal(days, chart.Count); Assert.Equal(100, chart.Sum(b => b.Volume));
    }

    [Fact]
    public async Task CsvImportsOverThousandRowsAndOptionalColumns()
    {
        await WithDatabase(async db => {
            var csv = "Date,ExerciseName\n" + string.Concat(Enumerable.Repeat("2026-10-05T12:00:00,Curl\n", 1501));
            var result = await new CsvImportService(db, Backend()).ImportFromCsvAsync(csv, "alice", "Alice");
            Assert.Empty(result.Errors); Assert.Equal(1, result.Imported);
            var workout = Assert.Single(await db.GetWorkoutsAsync("alice"));
            using var data = JsonDocument.Parse(workout.ExerciseDataJson);
            Assert.Equal(1501, data.RootElement.GetArrayLength());
        });
    }

    [Fact]
    public async Task CsvImportsMoreThanFiveSessionsAndTheirVolume()
    {
        await WithDatabase(async db => {
            var csv = "Date,WorkoutName,ExerciseName,Sets,Reps,WeightKg\n" + string.Concat(Enumerable.Range(1, 12).Select(i => $"2026-10-{i:00}T12:00:00,Day {i},Curl,1,10,20\n"));
            var result = await new CsvImportService(db, Backend()).ImportFromCsvAsync(csv, "alice", "Alice");
            Assert.Empty(result.Errors); Assert.Equal(12, result.Imported);
            Assert.All(await db.GetWorkoutsAsync("alice"), workout => Assert.Equal(200, workout.Volume));
        });
    }

    [Fact]
    public async Task CsvUploadUsesTheSameStableIdAndOwnerAsTheLocalWorkout()
    {
        string? uploadedId = null;
        var backend = new PocketBaseService(new ReplyFactory(async request => {
            if (request.RequestUri!.AbsolutePath.EndsWith("auth-with-password"))
                return new(System.Net.HttpStatusCode.OK) { Content = new StringContent("{\"token\":\"token\",\"record\":{\"id\":\"alice\"}}") };
            Assert.Equal(HttpMethod.Post, request.Method);
            using var payload = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
            uploadedId = payload.RootElement.GetProperty("id").GetString();
            Assert.Equal("alice", payload.RootElement.GetProperty("user").GetString());
            return new(System.Net.HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(new { id = uploadedId, user = "alice" })) };
        }), new BuildSecrets());
        await backend.LoginAsync("alice@example.invalid", "test-password");
        await WithDatabase(async db => {
            var result = await new CsvImportService(db, backend).ImportFromCsvAsync("Date,ExerciseName\n2026-10-05T12:00:00,Curl\n", "alice", "Alice");
            Assert.Empty(result.Errors);
            var workout = Assert.Single(await db.GetWorkoutsAsync("alice"));
            Assert.Equal(WorkoutSyncPayload.RecordId(workout.Id), uploadedId);
            Assert.False(workout.PendingSync);
        });
    }
    private sealed class ReplyFactory(Func<HttpRequestMessage, Task<HttpResponseMessage>> reply) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new ReplyHandler(reply));
    }
    private sealed class ReplyHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> reply) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => reply(request);
    }

    [Fact]
    public async Task CsvLimitCountsUtf8BytesAndChecksStreamBoundary()
    {
        using var exact = new MemoryStream(new byte[CsvFileLimit.MaxBytes]);
        Assert.Equal(CsvFileLimit.MaxBytes, (await CsvFileLimit.ReadAsync(exact)).Length);
        using var tooLarge = new MemoryStream(new byte[CsvFileLimit.MaxBytes + 1]);
        await Assert.ThrowsAsync<InvalidDataException>(() => CsvFileLimit.ReadAsync(tooLarge));
        await WithDatabase(async db => {
            var csv = new string('è', CsvFileLimit.MaxBytes / 2 + 1);
            Assert.True(csv.Length < CsvFileLimit.MaxBytes);
            var result = await new CsvImportService(db, Backend()).ImportFromCsvAsync(csv, "alice", "Alice");
            Assert.Equal(CsvFileLimit.ErrorMessage, Assert.Single(result.Errors));
            Assert.Empty(await db.GetWorkoutsAsync("alice"));
        });
    }

    [Fact]
    public async Task NonSeekableCsvCannotBypassByteLimit()
    {
        using var stream = new NonSeekable(new byte[CsvFileLimit.MaxBytes + 1]);
        await Assert.ThrowsAsync<InvalidDataException>(() => CsvFileLimit.ReadAsync(stream));
    }
    private sealed class NonSeekable(byte[] data) : MemoryStream(data) { public override bool CanSeek => false; }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BackgroundAndMinimizePreserveRunningOrExplicitlyPausedRest(bool pause)
    {
        await WithDatabase(async db => {
            var backend = Backend(); var notifications = new NotificationService(backend);
            var session = new WorkoutSession(db, backend); session.Start("Upper"); session.Minimize();
            notifications.ScheduleRest(600); if (pause) notifications.ToggleRestPause();
            session.BackgroundHandler = () => db.SaveWorkoutAsync(new() { Id = "draft", UserId = "alice", IsDraft = true, DraftRestSeconds = notifications.RestRemaining });
            var before = notifications.RestRemaining;
            await session.OnBackgroundAsync();
            await Task.Delay(1200);
            Assert.Equal(pause, notifications.IsRestPaused);
            if (pause) Assert.Equal(before, notifications.RestRemaining); else Assert.True(notifications.RestRemaining < before);
            Assert.True(session.ShowMiniBar); Assert.False(session.IsSuspended);
            Assert.Single(await db.GetWorkoutsAsync("alice")); notifications.CancelRest();
        });
    }

    private static async Task WithDatabase(Func<DatabaseService, Task> action)
    {
        var path = Path.Combine(Path.GetTempPath(), $"forge-november-{Guid.NewGuid():N}.db");
        try { await action(new DatabaseService(path)); }
        finally { await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync(); File.Delete(path); }
    }
}
