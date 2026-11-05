using Forge.Models;

namespace Forge.Services;

public static class WorkoutSetEditor
{
    public static void Remove(WorkoutExercise exercise, ExerciseSet set)
    {
        if (!exercise.Sets.Remove(set)) return;
        for (var index = 0; index < exercise.Sets.Count; index++)
            exercise.Sets[index].SetNumber = index + 1;
        exercise.IsCompleted = exercise.Sets.Count > 0 && exercise.Sets.All(WorkoutCompletion.IsLoggedSet);
    }
}
