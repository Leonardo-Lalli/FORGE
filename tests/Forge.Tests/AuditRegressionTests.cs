using Forge.Services;
using Forge.ViewModels;

namespace Forge.Tests;

public class AuditRegressionTests
{
    private static LocalWorkout Workout(int id, DateTime date, int minutes = 60) => new()
    {
        Id = id.ToString(), Date = date.ToString("O"), Duration = minutes,
        ExercisesJson = "[\"Squat\"]", Volume = 100
    };

    [Fact]
    public void EmptyAndDraftHistoryDoesNotUnlockAnything()
    {
        Assert.Empty(AchievementEvaluator.Evaluate([]));
        var draft = Workout(1, DateTime.Today);
        draft.IsDraft = true;
        Assert.Empty(AchievementEvaluator.Evaluate([draft]));
    }

    [Fact]
    public void RepeatedSessionsOnOneDayDoNotCreateDailyStreak()
    {
        var progress = AchievementEvaluator.Evaluate(Enumerable.Range(1, 30).Select(i => Workout(i, new(2026, 10, 8, 12, 0, 0))));
        Assert.Equal(30, progress["member"]);
        Assert.Equal(1, progress["unstoppable"]);
    }

    [Fact]
    public void ConsecutiveDaysAcrossMonthBoundaryCountOncePerDay()
    {
        var history = Enumerable.Range(0, 7).Select(i => Workout(i, new DateTime(2026, 9, 28, 12, 0, 0).AddDays(i))).ToList();
        history.Add(history[0]);
        Assert.Equal(7, AchievementEvaluator.Evaluate(history)["holy_week"]);
        Assert.Equal(7, AchievementEvaluator.Evaluate(history)["member"]);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(100, 1)]
    [InlineData(5999, 99)]
    [InlineData(6000, 100)]
    public void DurationIsConvertedFromMinutesToHours(int minutes, int hours)
    {
        var progress = AchievementEvaluator.Evaluate([Workout(1, DateTime.Today, minutes)]);
        Assert.Equal(hours, progress["milestone"]);
        Assert.Equal(hours, progress["hour_grinder"]);
    }

    [Theory]
    [InlineData("[]", 0)]
    [InlineData("null", 0)]
    [InlineData("invalid", 0)]
    [InlineData("[\"\"]", 0)]
    [InlineData("[\"base64-photo\"]", 1)]
    public void PhotoAwardRequiresARealPhotoEntry(string photos, int expected)
    {
        var workout = Workout(1, DateTime.Today);
        workout.PhotosJson = photos;
        Assert.Equal(expected, AchievementEvaluator.Evaluate([workout])["narcissus"]);
    }

    [Fact]
    public void WeekendRequiresSaturdayAndSundayInTheSameWeek()
    {
        var saturday = Workout(1, new(2026, 10, 3, 12, 0, 0));
        var nextSunday = Workout(2, new(2026, 10, 11, 12, 0, 0));
        Assert.Equal(1, AchievementEvaluator.Evaluate([saturday, nextSunday])["weekend_warrior"]);
        nextSunday.Date = new DateTime(2026, 10, 4, 12, 0, 0).ToString("O");
        Assert.Equal(2, AchievementEvaluator.Evaluate([saturday, nextSunday])["weekend_warrior"]);
    }

    [Fact]
    public void ExerciseVarietyIsAccumulatedAcrossHistoryCaseInsensitively()
    {
        var first = Workout(1, DateTime.Today);
        var second = Workout(2, DateTime.Today);
        second.ExercisesJson = "[\"squat\",\"Bench\",\"Bench\"]";
        var progress = AchievementEvaluator.Evaluate([first, second]);
        Assert.Equal(2, progress["encyclopedia"]);
        Assert.Equal(2, progress["explorer"]);
    }

    [Fact]
    public void TimeAwardsUseWorkoutTimeNotImportTime()
    {
        var progress = AchievementEvaluator.Evaluate([Workout(1, new DateTime(2020, 1, 1, 6, 0, 0, DateTimeKind.Local))]);
        Assert.Equal(1, progress["early_bird"]);
        Assert.Equal(0, progress["night_owl"]);
    }

    [Fact]
    public void InvalidDateDoesNotCountTowardAwardsOrXp()
    {
        var workout = new LocalWorkout { Id = "a", Date = "invalid" };
        Assert.Empty(AchievementEvaluator.Evaluate([workout]));
        Assert.Equal(0, TrainingProgress.FromWorkouts([workout]).Experience);
    }

    [Fact]
    public void XpIsIdempotentAndIgnoresDrafts()
    {
        var workouts = Enumerable.Range(1, 5).Select(i => Workout(i, DateTime.Today)).ToList();
        workouts.Add(workouts[0]);
        var draft = Workout(6, DateTime.Today); draft.IsDraft = true; workouts.Add(draft);
        var result = TrainingProgress.FromWorkouts(workouts);
        Assert.Equal(new TrainingProgress(500, 2, 0), result);
    }

    [Fact]
    public void SyncPayloadRetainsPhotosAndStablePocketBaseId()
    {
        var workout = Workout(1, DateTime.Today);
        workout.PhotosJson = "[\"photo\"]";
        var payload = WorkoutSyncPayload.Create(workout);
        Assert.Matches("^[a-z0-9]{15}$", (string)payload["id"]!);
        Assert.Equal(payload["id"], WorkoutSyncPayload.Create(workout)["id"]);
        Assert.Equal(["photo"], Assert.IsType<List<string>>(payload["photos"]));
        Assert.Equal("abcdefghijk1234", WorkoutSyncPayload.RecordId("abcdefghijk1234"));
    }

    [Fact]
    public void UnlockedCardsStayVisibleAndProgressIsClamped()
    {
        var card = new AchievementDisplay { IsUnlocked = true, Progress = 15, MaxProgress = 5 };
        Assert.Equal(1, card.CardOpacity);
        Assert.Equal(1, card.ProgressPercent);
    }

    [Fact]
    public void BaseViewModelTransitionsAreMutuallyExclusive()
    {
        var vm = new StateProbe();
        vm.Load(); Assert.True(vm.IsBusy); Assert.False(vm.HasData); Assert.False(vm.IsEmptyState);
        vm.Success(false); Assert.True(vm.IsEmptyState); Assert.False(vm.IsBusy); Assert.False(vm.HasData);
        vm.Error(); Assert.True(vm.HasError); Assert.False(vm.IsEmptyState); Assert.False(vm.IsBusy);
        vm.Success(true); Assert.True(vm.HasData); Assert.False(vm.HasError);
    }

    private sealed class StateProbe : BaseViewModel
    {
        public void Load() => SetLoading();
        public void Success(bool data) => SetSuccess(data);
        public void Error() => SetError("Expected failure");
    }
}

