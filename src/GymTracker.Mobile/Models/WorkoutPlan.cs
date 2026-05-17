using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

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
    public string ImageUrl { get; set; } = string.Empty;
    public string GifUrl { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<string> Instructions { get; set; } = new();
    public int Order { get; set; }
    public int RestSeconds { get; set; }
    public ObservableCollection<ExerciseSet> Sets { get; set; } = new();
    public bool IsCompleted { get; set; }
}

public partial class ExerciseSet : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int SetNumber { get; set; }

    [ObservableProperty] private double weightKg;
    [ObservableProperty] private int reps;
    [ObservableProperty] private bool isCompleted;
}
