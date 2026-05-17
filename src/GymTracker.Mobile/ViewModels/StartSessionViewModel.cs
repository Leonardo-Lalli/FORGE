using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class ProtocolCard : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ExerciseCount { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string Intensity { get; set; } = string.Empty;
    public string IntensityColor { get; set; } = "Primary";
}

public partial class StartSessionViewModel : BaseViewModel
{
    [ObservableProperty] private ObservableCollection<ProtocolCard> protocols = new();

    public StartSessionViewModel()
    {
        HasData = true;
        SeedMockPlansIfEmpty();
        LoadProtocols();
    }

    private static void SeedMockPlansIfEmpty()
    {
        var existing = PlanStore.LoadPlans();
        if (existing.Count > 0) return;

        var plans = new List<WorkoutPlan>
        {
            new()
            {
                Name = "Push Power",
                Exercises = new List<WorkoutExercise>
                {
                    new() { ExerciseName = "Bench Press", BodyPart = "Chest", Order = 1, Sets = { new() { SetNumber = 1, WeightKg = 60, Reps = 10 }, new() { SetNumber = 2, WeightKg = 70, Reps = 8 }, new() { SetNumber = 3, WeightKg = 80, Reps = 6 } } },
                    new() { ExerciseName = "Incline Dumbbell Press", BodyPart = "Upper Chest", Order = 2, Sets = { new() { SetNumber = 1, WeightKg = 25, Reps = 12 }, new() { SetNumber = 2, WeightKg = 27.5, Reps = 10 }, new() { SetNumber = 3, WeightKg = 30, Reps = 8 } } },
                    new() { ExerciseName = "Lateral Raises", BodyPart = "Shoulders", Order = 3, Sets = { new() { SetNumber = 1, WeightKg = 10, Reps = 15 }, new() { SetNumber = 2, WeightKg = 12, Reps = 12 }, new() { SetNumber = 3, WeightKg = 12, Reps = 10 } } },
                    new() { ExerciseName = "Tricep Pushdowns", BodyPart = "Triceps", Order = 4, Sets = { new() { SetNumber = 1, WeightKg = 20, Reps = 15 }, new() { SetNumber = 2, WeightKg = 22.5, Reps = 12 }, new() { SetNumber = 3, WeightKg = 25, Reps = 10 } } }
                },
                RestSeconds = 90
            },
            new()
            {
                Name = "Leg Day Protocol",
                Exercises = new List<WorkoutExercise>
                {
                    new() { ExerciseName = "Barbell Squat", BodyPart = "Quads", Order = 1, Sets = { new() { SetNumber = 1, WeightKg = 80, Reps = 8 }, new() { SetNumber = 2, WeightKg = 90, Reps = 6 }, new() { SetNumber = 3, WeightKg = 100, Reps = 4 }, new() { SetNumber = 4, WeightKg = 110, Reps = 3 } } },
                    new() { ExerciseName = "Romanian Deadlift", BodyPart = "Hamstrings", Order = 2, Sets = { new() { SetNumber = 1, WeightKg = 60, Reps = 12 }, new() { SetNumber = 2, WeightKg = 70, Reps = 10 }, new() { SetNumber = 3, WeightKg = 80, Reps = 8 } } },
                    new() { ExerciseName = "Leg Press", BodyPart = "Quads", Order = 3, Sets = { new() { SetNumber = 1, WeightKg = 150, Reps = 12 }, new() { SetNumber = 2, WeightKg = 170, Reps = 10 }, new() { SetNumber = 3, WeightKg = 190, Reps = 8 } } },
                    new() { ExerciseName = "Leg Extensions", BodyPart = "Quads", Order = 4, Sets = { new() { SetNumber = 1, WeightKg = 50, Reps = 15 }, new() { SetNumber = 2, WeightKg = 55, Reps = 12 }, new() { SetNumber = 3, WeightKg = 60, Reps = 10 } } },
                    new() { ExerciseName = "Calf Raises", BodyPart = "Calves", Order = 5, Sets = { new() { SetNumber = 1, WeightKg = 100, Reps = 20 }, new() { SetNumber = 2, WeightKg = 110, Reps = 15 }, new() { SetNumber = 3, WeightKg = 120, Reps = 12 } } }
                },
                RestSeconds = 120
            },
            new()
            {
                Name = "Core Stabilization",
                Exercises = new List<WorkoutExercise>
                {
                    new() { ExerciseName = "Plank", BodyPart = "Core", Order = 1, Sets = { new() { SetNumber = 1, WeightKg = 0, Reps = 60 }, new() { SetNumber = 2, WeightKg = 0, Reps = 45 } } },
                    new() { ExerciseName = "Russian Twists", BodyPart = "Obliques", Order = 2, Sets = { new() { SetNumber = 1, WeightKg = 10, Reps = 20 }, new() { SetNumber = 2, WeightKg = 10, Reps = 20 }, new() { SetNumber = 3, WeightKg = 12, Reps = 15 } } },
                    new() { ExerciseName = "Hanging Leg Raises", BodyPart = "Lower Abs", Order = 3, Sets = { new() { SetNumber = 1, WeightKg = 0, Reps = 15 }, new() { SetNumber = 2, WeightKg = 0, Reps = 12 } } }
                },
                RestSeconds = 60
            }
        };

        foreach (var plan in plans)
            PlanStore.SavePlan(plan);
    }

    public void LoadProtocols()
    {
        var plans = PlanStore.LoadPlans();
        Protocols.Clear();
        foreach (var plan in plans.Take(3))
        {
            Protocols.Add(new ProtocolCard
            {
                Id = plan.Id,
                Name = plan.Name,
                ExerciseCount = plan.Exercises.Count,
                Duration = $"{plan.Exercises.Count * 5 + plan.RestSeconds / 60 * plan.Exercises.Sum(e => e.Sets.Count)}m",
                Intensity = plan.Exercises.Count switch
                {
                    >= 6 => "High",
                    >= 4 => "Medium",
                    _ => "Low"
                },
                IntensityColor = plan.Exercises.Count switch
                {
                    >= 6 => "LimeGreen",
                    >= 4 => "Primary",
                    _ => "TextSecondary"
                }
            });
        }
    }

    [RelayCommand]
    private async Task QuickStartAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "free"
        });
    }

    [RelayCommand]
    private async Task CreateNewPlanAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "create"
        });
    }

    [RelayCommand]
    private async Task StartProtocolAsync(ProtocolCard protocol)
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "plan",
            ["planId"] = protocol.Id
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void DeleteProtocol(ProtocolCard protocol)
    {
        PlanStore.DeletePlan(protocol.Id);
        LoadProtocols();
    }
}
