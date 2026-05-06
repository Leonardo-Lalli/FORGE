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
    private CancellationTokenSource? elapsedCts;
    private int restSecondsRemaining;
    private DateTime workoutStartTime;

    [ObservableProperty] private string mode = "free";
    [ObservableProperty] private string planId = string.Empty;
    [ObservableProperty] private string planName = "Nuovo Allenamento";
    [ObservableProperty] private ObservableCollection<WorkoutExercise> exercises = new();
    [ObservableProperty] private WorkoutExercise? selectedExercise;
    [ObservableProperty] private int restDuration = 90;
    [ObservableProperty] private bool isRestTimerActive;
    [ObservableProperty] private string restTimerText = "90s";
    [ObservableProperty] private double restTimerProgress = 1.0;
    [ObservableProperty] private string elapsedTime = "00:00:00";
    [ObservableProperty] private int totalSetsCompleted;
    [ObservableProperty] private string notificationMessage = string.Empty;
    [ObservableProperty] private string restTimerLabel = "pausa";
    [ObservableProperty] private bool isNotificationVisible;

    partial void OnModeChanged(string value)
    {
        PlanName = value switch { "free" => "Allenamento Libero", "create" => "Nuova Scheda", _ => "Scheda Salvata" };
        HasData = true;
        IsEmptyState = false;
        workoutStartTime = DateTime.Now;
        _ = RunElapsedTimerAsync();
    }

    partial void OnNotificationMessageChanged(string value)
    {
        IsNotificationVisible = !string.IsNullOrWhiteSpace(value);
    }

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
    }

    [RelayCommand]
    private void RemoveExercise(WorkoutExercise exercise)
    {
        Exercises.Remove(exercise);
        for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1;
    }

    [RelayCommand]
    private void AddSet(WorkoutExercise exercise)
    {
        var setNum = exercise.Sets.Count + 1;
        var last = exercise.Sets.LastOrDefault();
        exercise.Sets.Add(new ExerciseSet { SetNumber = setNum, WeightKg = last?.WeightKg ?? 20, Reps = last?.Reps ?? 10 });
    }

    [RelayCommand]
    private void RemoveSet(ExerciseSet set)
    {
        var parent = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        parent?.Sets.Remove(set);
        for (int i = 0; i < parent?.Sets.Count; i++) parent.Sets[i].SetNumber = i + 1;
    }

    [RelayCommand]
    private void IncrementReps(ExerciseSet set) { set.Reps++; OnPropertyChanged(nameof(set)); }

    [RelayCommand]
    private void DecrementReps(ExerciseSet set) { if (set.Reps > 1) set.Reps--; OnPropertyChanged(nameof(set)); }

    [RelayCommand]
    private void IncrementWeight(ExerciseSet set) { set.WeightKg += 2.5; OnPropertyChanged(nameof(set)); }

    [RelayCommand]
    private void DecrementWeight(ExerciseSet set) { if (set.WeightKg >= 2.5) set.WeightKg -= 2.5; OnPropertyChanged(nameof(set)); }

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
        if (idx > 0) { Exercises.Move(idx, idx - 1); for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1; }
    }

    [RelayCommand]
    private void MoveExerciseDown(WorkoutExercise exercise)
    {
        var idx = Exercises.IndexOf(exercise);
        if (idx < Exercises.Count - 1) { Exercises.Move(idx, idx + 1); for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1; }
    }

    [RelayCommand]
    private void ReplaceExercise(WorkoutExercise exercise)
    {
        ShowNotification($"Sostituisci {exercise.ExerciseName} — seleziona dal catalogo");
    }

    [RelayCommand]
    private void StartExerciseRestTimer(WorkoutExercise exercise)
    {
        restCts?.Cancel();
        restCts = new CancellationTokenSource();
        var token = restCts.Token;
        restSecondsRemaining = RestDuration;
        IsRestTimerActive = true;
        RestTimerLabel = $"pausa {exercise.ExerciseName}";
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
                    if (restSecondsRemaining is <= 5 and > 0)
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                if (!token.IsCancellationRequested)
                {
                    IsRestTimerActive = false;
                    ShowNotification($"⏰ Pausa finita per {exercise.ExerciseName}!");
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                }
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    private void StartRestTimer()
    {
        restCts?.Cancel();
        restCts = new CancellationTokenSource();
        var token = restCts.Token;
        restSecondsRemaining = RestDuration;
        IsRestTimerActive = true;
        RestTimerLabel = "pausa automatica";
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
                    if (restSecondsRemaining is <= 5 and > 0)
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                if (!token.IsCancellationRequested)
                {
                    IsRestTimerActive = false;
                    ShowNotification("⏰ Pausa finita! Tempo di spingere!");
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
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
        ShowNotification($"Scheda \"{PlanName}\" salvata!");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task FinishWorkoutAsync()
    {
        elapsedCts?.Cancel();
        ShowNotification($"Allenamento completato! {TotalSetsCompleted} serie, {Exercises.Count} esercizi.");
        await Shell.Current.GoToAsync("..");
    }

    private void ShowNotification(string message)
    {
        NotificationMessage = message;
    }

    private async Task RunElapsedTimerAsync()
    {
        elapsedCts?.Cancel();
        elapsedCts = new CancellationTokenSource();
        var token = elapsedCts.Token;
        try
        {
            while (!token.IsCancellationRequested)
            {
                var elapsed = DateTime.Now - workoutStartTime;
                ElapsedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                await Task.Delay(1000, token);
            }
        }
        catch (TaskCanceledException) { }
    }
}
