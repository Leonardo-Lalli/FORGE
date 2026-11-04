using Forge.Services;

namespace Forge.Tests.Services;

public class WorkoutRecoveryTests
{
    [Fact]
    public void OnlyPerformedExercisesCountIncludingBodyweight()
    {
        var empty = new WorkoutExercise { ExerciseName = "Empty" };
        var suggested = new WorkoutExercise { ExerciseName = "Suggested", Sets = [new() { WeightKg = 50, Reps = 10, IsSuggested = true }] };
        var checkedEmpty = new WorkoutExercise { ExerciseName = "No reps", Sets = [new() { IsCompleted = true }] };
        var bodyweight = new WorkoutExercise { ExerciseName = "Push up", Sets = [new() { Reps = 12, IsCompleted = true }] };
        Assert.Same(bodyweight, Assert.Single(WorkoutCompletion.LoggedExercises([empty, suggested, checkedEmpty, bodyweight])));
    }

    [Theory]
    [InlineData(1, 0, 1, 0)]
    [InlineData(1, 125, 2, 125)]
    [InlineData(0, 0, 3, 180)]
    public void RecoveryUsesExactSavedTimeAndSupportsLegacyDrafts(int version, int seconds, int minutes, int expected)
        => Assert.Equal(expected, WorkoutCompletion.ElapsedSeconds(new() { DraftStateVersion = version, DraftElapsedSeconds = seconds, Duration = minutes }));

    [Fact]
    public async Task DraftSurvivesNewConnectionWithNotesPhotosAndPausedTimers()
    {
        var path = Path.Combine(Path.GetTempPath(), $"forge-recovery-{Guid.NewGuid():N}.db");
        try
        {
            var db = new DatabaseService(path);
            await db.SaveWorkoutAsync(new() { Id = "draft", UserId = "alice", IsDraft = true, DraftStateVersion = 1,
                DraftElapsedSeconds = 125, DraftRestSeconds = 43, Notes = "First line\nSecond line", PhotosJson = "[\"photo\"]",
                ExerciseDataJson = "[{\"name\":\"Squat\",\"sets\":[{\"reps\":10,\"weightKg\":50,\"isCompleted\":true}]}]" });
            var reopened = new DatabaseService(path);
            var draft = Assert.Single(await reopened.GetWorkoutsAsync("alice"));
            Assert.Equal(125, WorkoutCompletion.ElapsedSeconds(draft));
            Assert.Equal(43, draft.DraftRestSeconds);
            Assert.Equal("First line\nSecond line", draft.Notes);
            Assert.Equal("[\"photo\"]", draft.PhotosJson);
            Assert.Contains("Squat", draft.ExerciseDataJson);
            Assert.Empty(await reopened.GetPendingSyncAsync());
            Assert.Empty(await reopened.GetWorkoutsAsync("bob"));
            // Metadata recovery uses the current owner's draft without opening the editor or starting a clock.
            var pb = new PocketBaseService(null!, new BuildSecrets());
            var session = new WorkoutSession(reopened, pb);
            await session.RecoverAsync();
            Assert.False(session.IsActive); // Alice's draft is not visible to a different owner.
            draft.UserId = "";
            draft.Name = "Recovered session";
            await reopened.SaveWorkoutAsync(draft);
            await session.RecoverAsync();
            Assert.True(session.ShowMiniBar);
            Assert.True(session.IsSuspended);
            Assert.Equal("00:02:05", session.ElapsedTime);
            Assert.Equal("Recovered session", session.MiniLabel);
            Assert.Contains("00:43", session.RestTimerText);
            Assert.Empty(session.RecoveryError);
        }
        finally
        {
            await new SQLite.SQLiteAsyncConnection(path, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache).CloseAsync();
            File.Delete(path);
        }
    }

    [Fact]
    public void MiniBarTracksSessionTransitionsAndUpdatesBindings()
    {
        var session = new WorkoutSession(null!, null!);
        var changed = new List<string?>();
        session.PropertyChanged += (_, args) => changed.Add(args.PropertyName);
        session.Start("Leg day");
        Assert.False(session.ShowMiniBar);
        session.Minimize();
        session.UpdateElapsed("00:02:05");
        session.UpdateRestTimer(true, "00:43");
        Assert.True(session.ShowMiniBar);
        Assert.Equal("Leg day", session.MiniLabel);
        Assert.Equal("Recupero · 00:43", session.MiniRestLabel);
        Assert.DoesNotContain("00:02:05", session.MiniRestLabel);
        Assert.Contains(nameof(WorkoutSession.ShowMiniBar), changed);
        Assert.Contains(nameof(WorkoutSession.MiniLabel), changed);
        session.Restore(); Assert.False(session.ShowMiniBar);
        session.End(); Assert.False(session.IsActive); Assert.False(session.IsRestTimerActive);
        Assert.Equal("Nessun recupero attivo", session.MiniRestLabel);
    }
}
