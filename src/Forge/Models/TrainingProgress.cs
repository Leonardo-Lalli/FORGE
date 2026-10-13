namespace Forge.Models;

public record TrainingProgress(long Experience, long Level, double LevelProgress)
{
    public static TrainingProgress FromAchievements(int unlocked)
    {
        var xp = Math.Max(0, unlocked) * 100L;
        return new(xp, xp / 500 + 1, xp % 500 / 500.0);
    }
    // Recomputed from persisted workout IDs so reloads never grant XP twice.
    public static TrainingProgress FromWorkouts(IEnumerable<LocalWorkout> workouts)
    {
        var count = workouts.Where(w => !w.IsDraft && !string.IsNullOrWhiteSpace(w.Id)
            && DateTimeOffset.TryParse(w.Date, out _)).DistinctBy(w => w.Id).LongCount();
        var xp = count * 100;
        return new(xp, xp / 500 + 1, xp % 500 / 500.0);
    }
}
