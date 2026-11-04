using Forge.Models;

namespace Forge.Services;

public static class WorkoutCompletion
{
    // A checked set with repetitions counts even for bodyweight movements (zero external weight).
    public static bool IsLoggedSet(ExerciseSet set) => set.IsCompleted && set.Reps > 0;
    public static List<WorkoutExercise> LoggedExercises(IEnumerable<WorkoutExercise> exercises)
        => exercises.Where(e => e.Sets.Any(IsLoggedSet)).ToList();
    public static int ElapsedSeconds(LocalWorkout draft)
        => Math.Max(0, draft.DraftStateVersion > 0 ? draft.DraftElapsedSeconds : draft.Duration * 60);
}
