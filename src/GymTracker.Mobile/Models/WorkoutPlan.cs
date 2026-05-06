namespace GymTracker.Mobile.Models;

public class WorkoutPlan
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<WorkoutExercise> Exercises { get; set; } = new();
    public int RestSeconds { get; set; } = 90;
}

public class WorkoutExercise
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<ExerciseSet> Sets { get; set; } = new();
    public bool IsCompleted { get; set; }
}

public class ExerciseSet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int SetNumber { get; set; }
    public double WeightKg { get; set; }
    public int Reps { get; set; }
    public bool IsCompleted { get; set; }
}
