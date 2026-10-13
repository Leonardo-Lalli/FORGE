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
    [ObservableProperty] private string imageUrl = string.Empty;
    [ObservableProperty] private string gifUrl = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(ExecutionTip))] private List<string> instructions = new();
    [ObservableProperty, NotifyPropertyChangedFor(nameof(ExecutionTip))] private int executionTipIndex;
    public string ExecutionTip => Instructions.Count == 0 ? "Istruzioni non disponibili per questo esercizio." : $"STEP {ExecutionTipIndex % Instructions.Count + 1}/{Instructions.Count} · {Instructions[ExecutionTipIndex % Instructions.Count]}";
    public void AdvanceExecutionTip() { if (Instructions.Count > 1) ExecutionTipIndex = (ExecutionTipIndex + 1) % Instructions.Count; }
    [ObservableProperty] private int order;
    public string RestLabel => $"Recupero {RestSeconds / 60:00}:{RestSeconds % 60:00}";
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
        OnPropertyChanged(nameof(RestLabel));
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
    [ObservableProperty, NotifyPropertyChangedFor(nameof(DisplayLabel))] private int setNumber;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(DisplayLabel))] private string kind = "normal";
    public string DisplayLabel => Kind switch { "warmup" => "W", "drop" => "D", "failure" => "F", _ => SetNumber.ToString() };


    [ObservableProperty] private double weightKg;
    [ObservableProperty] private int reps;
    [ObservableProperty] private bool isCompleted;
    [ObservableProperty] private bool isSuggested;
    [ObservableProperty] private string previousText = "—";
    partial void OnWeightKgChanged(double value) => IsSuggested = false;
    partial void OnRepsChanged(int value) => IsSuggested = false;
}
