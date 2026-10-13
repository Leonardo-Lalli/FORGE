using Forge.Services;

namespace Forge.Tests.Services;

public class StatsAndExerciseProgressTests
{
    [Theory]
    [InlineData("week", 2026, 10, 12)]
    [InlineData("month", 2026, 10, 1)]
    [InlineData("3months", 2026, 8, 1)]
    [InlineData("year", 2025, 11, 1)]
    public void PeriodUsesCalendarBoundaries(string filter, int year, int month, int day)
        => Assert.Equal(new DateTime(year, month, day), StatsPeriod.Start(filter, new DateTime(2026, 10, 14)));

    [Theory]
    [InlineData(0, 60)]
    [InlineData(1, 2)]
    [InlineData(2, 18)]
    [InlineData(3, 980)]
    public void MetricsOnlyUseCompletedSetsFromFinishedMatchingWorkouts(int metric, double expected)
    {
        var json = """[{"exerciseId":"bench","name":"Bench","sets":[{"weightKg":50,"reps":10,"isCompleted":true},{"weightKg":60,"reps":8,"isCompleted":true},{"weightKg":100,"reps":20,"isCompleted":false}]}]""";
        var history = new[] {
            new LocalWorkout { Id="1", Date="2026-10-12T12:00:00Z", ExerciseDataJson=json },
            new LocalWorkout { Id="2", Date="2026-10-12T13:00:00Z", ExerciseDataJson=json, IsDraft=true }
        };
        var points = ExerciseProgress.Build(history, new WorkoutExercise { ExerciseId="bench", ExerciseName="Bench" }, metric);
        Assert.Equal(expected, Assert.Single(points).Value);
        Assert.Empty(ExerciseProgress.Build(history, new WorkoutExercise { ExerciseId="other", ExerciseName="Bench" }, metric));
    }

    [Fact]
    public void AchievementLevelMatchesUnlockedBadges()
    {
        var result = TrainingProgress.FromAchievements(7);
        Assert.Equal(700, result.Experience);
        Assert.Equal(2, result.Level);
        Assert.Equal(0.4, result.LevelProgress);
    }
}
