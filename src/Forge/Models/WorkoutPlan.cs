using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Forge.Models;

public class WorkoutPlan
{
    public string UserId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<WorkoutExercise> Exercises { get; set; } = new();
    public int RestSeconds { get; set; } = 90;
}

public partial class WorkoutExercise : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string GifUrl { get; set; } = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    public List<string> Instructions { get; set; } = new();
    public int Order { get; set; }
    [ObservableProperty] private int restSeconds = 90;
    [ObservableProperty] private int restMinutes = 1;
    [ObservableProperty] private int restSecondPart = 30;
    private bool updatingRest;
    partial void OnRestSecondsChanged(int value)
    {
        if (updatingRest) return;
        updatingRest = true;
        RestSeconds = Math.Clamp(value, 0, 300);
        RestMinutes = RestSeconds / 60;
        RestSecondPart = RestSeconds % 60;
        updatingRest = false;
    }
    partial void OnRestMinutesChanged(int value) => UpdateRestParts();
    partial void OnRestSecondPartChanged(int value) => UpdateRestParts();
    private void UpdateRestParts()
    {
        if (updatingRest) return;
        RestSeconds = Math.Clamp(RestMinutes * 60 + RestSecondPart, 0, 300);
        if (RestMinutes == 5 && RestSecondPart != 0) RestSecondPart = 0;
    }
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
    [ObservableProperty] private bool isSuggested;
    partial void OnWeightKgChanged(double value) => IsSuggested = false;
    partial void OnRepsChanged(int value) => IsSuggested = false;
}
