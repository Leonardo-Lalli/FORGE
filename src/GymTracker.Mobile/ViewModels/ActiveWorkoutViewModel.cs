using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Models;

namespace GymTracker.Mobile.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(PlanId), "planId")]
public partial class ActiveWorkoutViewModel : BaseViewModel
{
    private CancellationTokenSource? restCts;
    private int restSecondsRemaining;

    [ObservableProperty]
    private string mode = "free";

    [ObservableProperty]
    private string planId = string.Empty;

    [ObservableProperty]
    private string planName = "Nuovo Allenamento";

    [ObservableProperty]
    private ObservableCollection<WorkoutExercise> exercises = new();

    [ObservableProperty]
    private WorkoutExercise? selectedExercise;

    [ObservableProperty]
    private int restDuration = 90;

    [ObservableProperty]
    private bool isRestTimerActive;

    [ObservableProperty]
    private string restTimerText = "90s";

    [ObservableProperty]
    private double restTimerProgress = 1.0;

    [ObservableProperty]
    private string elapsedTime = "00:00";

    [ObservableProperty]
    private int totalSetsCompleted;

    [ObservableProperty]
    private string notificationMessage = string.Empty;

    private DateTime workoutStartTime;

    partial void OnModeChanged(string value)
    {
        if (value == "free") PlanName = "Allenamento Libero";
        else if (value == "create") PlanName = "Nuova Scheda";
        else if (value == "saved") PlanName = "Scheda Salvata";

        HasData = true;
        IsEmptyState = false;
        workoutStartTime = DateTime.Now;
        _ = UpdateElapsedTimer();
    }

    partial void OnSelectedExerciseChanged(WorkoutExercise? value)
    {
        if (value != null)
            OnPropertyChanged(nameof(IsExerciseSelected));
    }

    public bool IsExerciseSelected => SelectedExercise != null;

    [RelayCommand]
    private void AddExercise()
    {
        var order = Exercises.Count + 1;
        Exercises.Add(new WorkoutExercise
        {
            ExerciseName = $"Esercizio {order}",
            BodyPart = "Seleziona",
            Order = order
        });
        IsEmptyState = false;
    }

    [RelayCommand]
    private void RemoveExercise(WorkoutExercise exercise)
    {
        Exercises.Remove(exercise);
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].Order = i + 1;

        if (Exercises.Count == 0)
            IsEmptyState = true;
    }

    [RelayCommand]
    private void AddSet(WorkoutExercise exercise)
    {
        var setNum = exercise.Sets.Count + 1;
        var lastSet = exercise.Sets.LastOrDefault();
        exercise.Sets.Add(new ExerciseSet
        {
            SetNumber = setNum,
            WeightKg = lastSet?.WeightKg ?? 0,
            Reps = lastSet?.Reps ?? 10
        });
    }

    [RelayCommand]
    private void RemoveSet(WorkoutExercise exercise)
    {
        if (exercise.Sets.Count > 0)
            exercise.Sets.RemoveAt(exercise.Sets.Count - 1);
    }

    [RelayCommand]
    private void CompleteSet(ExerciseSet set)
    {
        set.IsCompleted = true;
        TotalSetsCompleted++;
        StartRestTimer();
    }

    [RelayCommand]
    private void MoveExerciseUp(WorkoutExercise exercise)
    {
        var idx = Exercises.IndexOf(exercise);
        if (idx > 0)
        {
            Exercises.Move(idx, idx - 1);
            for (int i = 0; i < Exercises.Count; i++)
                Exercises[i].Order = i + 1;
        }
    }

    [RelayCommand]
    private void MoveExerciseDown(WorkoutExercise exercise)
    {
        var idx = Exercises.IndexOf(exercise);
        if (idx < Exercises.Count - 1)
        {
            Exercises.Move(idx, idx + 1);
            for (int i = 0; i < Exercises.Count; i++)
                Exercises[i].Order = i + 1;
        }
    }

    [RelayCommand]
    private void ReplaceExercise(WorkoutExercise exercise)
    {
        ShowNotification($"Sostituisci {exercise.ExerciseName} - seleziona dal catalogo");
    }

    [RelayCommand]
    private void StartRestTimer()
    {
        restCts?.Cancel();
        restCts = new CancellationTokenSource();
        var token = restCts.Token;

        restSecondsRemaining = RestDuration;
        IsRestTimerActive = true;
        RestTimerText = $"{restSecondsRemaining}s";
        RestTimerProgress = 1.0;

        Task.Run(async () =>
        {
            try
            {
                while (restSecondsRemaining > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    restSecondsRemaining--;
                    RestTimerProgress = (double)restSecondsRemaining / RestDuration;
                    RestTimerText = $"{restSecondsRemaining}s";

                    if (restSecondsRemaining <= 5 && restSecondsRemaining > 0)
                        ShowNotification($"Pausa: {restSecondsRemaining}s rimanenti");
                }

                if (!token.IsCancellationRequested)
                {
                    IsRestTimerActive = false;
                    ShowNotification("Pausa finita! Tempo di spingere!");
                }
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    [RelayCommand]
    private void SkipRestTimer()
    {
        restCts?.Cancel();
        IsRestTimerActive = false;
        restSecondsRemaining = 0;
        RestTimerProgress = 0;
        RestTimerText = "0s";
    }

    [RelayCommand]
    private async Task SaveWorkoutPlanAsync()
    {
        if (string.IsNullOrWhiteSpace(PlanName))
        {
            ShowNotification("Dai un nome alla scheda prima di salvarla.");
            return;
        }

        var plan = new WorkoutPlan
        {
            Name = PlanName,
            RestSeconds = RestDuration,
            Exercises = Exercises.ToList()
        };

        ShowNotification($"Scheda \"{PlanName}\" salvata!");

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task FinishWorkoutAsync()
    {
        ShowNotification($"Allenamento completato! {TotalSetsCompleted} serie, {Exercises.Count} esercizi.");
        await Shell.Current.GoToAsync("..");
    }

    private void ShowNotification(string message)
    {
        NotificationMessage = message;
        OnPropertyChanged(nameof(NotificationMessage));
    }

    private async Task UpdateElapsedTimer()
    {
        while (IsBusy == false || HasData)
        {
            var elapsed = DateTime.Now - workoutStartTime;
            ElapsedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}";
            await Task.Delay(10000);
        }
    }
}
