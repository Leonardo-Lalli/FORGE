using Forge.Services;
namespace Forge.Tests.Services;
public class WorkoutFeedbackTests
{
    [Theory]
    [InlineData("chest", "pectorals", "chest", false)]
    [InlineData("upper body", "latissimus dorsi", "", true)]
    [InlineData("back", "lats", "back", true)]
    [InlineData("back", "biceps", "back", false)]
    [InlineData("back", "", "back", true)]
    public void Back_filter_uses_primary_muscle(string region, string primary, string category, bool expected)
        => Assert.Equal(expected, ExerciseMuscleFilter.Matches(region, primary, category, ["back", "lats", "latissimus dorsi"]));
    [Fact]
    public void Advice_rotates_and_handles_shorter_instructions()
    {
        var exercise = new WorkoutExercise { Instructions = ["Setup", "Lift", "Lower"] };
        Assert.Contains("Setup", exercise.ExecutionTip);
        exercise.AdvanceExecutionTip(); Assert.Contains("Lift", exercise.ExecutionTip);
        exercise.AdvanceExecutionTip(); exercise.AdvanceExecutionTip(); Assert.Contains("Setup", exercise.ExecutionTip);
        exercise.Instructions = []; Assert.Contains("non disponibili", exercise.ExecutionTip);
    }
}
