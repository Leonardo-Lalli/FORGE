using Forge.Services;
namespace Forge.Tests.Services;
public class PlanSessionChangesTests
{
    [Fact] public void Completion_and_suggestions_do_not_change_plan()
    {
        var ex = new WorkoutExercise { Sets = new([new ExerciseSet { SetNumber = 1, WeightKg = 40, Reps = 10 }]) };
        var baseline = PlanSessionChanges.Signature("Plan", "", 90, [ex]);
        ex.Sets[0].IsCompleted = true; ex.Sets[0].IsSuggested = true;
        Assert.Equal(baseline, PlanSessionChanges.Signature("Plan", "", 90, [ex]));
        ex.Sets[0].Kind = "warmup";
        Assert.NotEqual(baseline, PlanSessionChanges.Signature("Plan", "", 90, [ex]));
    }
    [Fact] public void Notes_alone_do_not_require_plan_confirmation()
    {
        var ex = new WorkoutExercise { Notes = "Old" };
        var baseline = PlanSessionChanges.Signature("Plan", "Old", 90, [ex]);
        ex.Notes = "New";
        Assert.Equal(baseline, PlanSessionChanges.Signature("Plan", "New", 90, [ex]));
        ex.RestSeconds = 120;
        Assert.NotEqual(baseline, PlanSessionChanges.Signature("Plan", "New", 90, [ex]));
    }
    [Fact] public void Rename_and_added_exercise_are_detected()
    {
        var ex = new WorkoutExercise { ExerciseName = "Squat" };
        var baseline = PlanSessionChanges.Signature("Plan", "", 90, [ex]);
        Assert.NotEqual(baseline, PlanSessionChanges.Signature("Renamed", "", 90, [ex]));
        Assert.NotEqual(baseline, PlanSessionChanges.Signature("Plan", "", 90, [ex, new WorkoutExercise()]));
    }
    [Fact] public void Copy_keeps_kind_and_resets_session_without_mutating_original()
    {
        var ex = new WorkoutExercise { Sets = new([new ExerciseSet { SetNumber = 1, Kind = "drop", IsCompleted = true }]) };
        var copy = PlanSessionChanges.CopyExercises([ex]);
        Assert.Equal("D", copy[0].Sets[0].DisplayLabel); Assert.False(copy[0].Sets[0].IsCompleted); Assert.True(ex.Sets[0].IsCompleted);
        copy[0].Sets[0].Kind = "normal"; Assert.Equal("D", ex.Sets[0].DisplayLabel);
    }
}
