using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(PlanId), "planId")]
public partial class ActiveWorkoutViewModel : BaseViewModel
{
    private readonly WorkoutSession workoutSession;
    private CancellationTokenSource? restCts;
    private CancellationTokenSource? elapsedCts;
    private int restSecondsRemaining;
    private DateTime workoutStartTime;

    [ObservableProperty] private string mode = "free";
    [ObservableProperty] private string planId = string.Empty;
    [ObservableProperty] private string planName = "Nuovo Allenamento";
    [ObservableProperty] private ObservableCollection<WorkoutExercise> exercises = new();
    [ObservableProperty] private int restDuration = 90;
    [ObservableProperty] private bool isRestTimerActive;
    [ObservableProperty] private string restTimerText = "90s";
    [ObservableProperty] private double restTimerProgress = 1.0;
    [ObservableProperty] private string elapsedTime = "00:00:00";
    [ObservableProperty] private int totalSetsCompleted;
    [ObservableProperty] private string notificationMessage = string.Empty;
    [ObservableProperty] private string restTimerLabel = "pausa";
    [ObservableProperty] private bool isNotificationVisible;
    [ObservableProperty] private bool isTimerRunning;
    [ObservableProperty] private bool isCreating; // true = modalità creazione scheda

    public ActiveWorkoutViewModel(WorkoutSession workoutSession)
    {
        this.workoutSession = workoutSession;
    }

    partial void OnModeChanged(string value)
    {
        PlanName = value switch { "free" => "Allenamento Libero", "create" => "Nuova Scheda", _ => "Scheda Salvata" };
        IsCreating = (value == "create");

        HasData = true;
        workoutStartTime = DateTime.Now;

        if (!IsCreating)
        {
            IsTimerRunning = true;
            workoutSession.Start(PlanName);
            _ = RunElapsedTimerAsync();
        }
    }

    partial void OnNotificationMessageChanged(string value)
    {
        IsNotificationVisible = !string.IsNullOrWhiteSpace(value);
    }

    [RelayCommand]
    private void StartWorkout()
    {
        IsCreating = false;
        IsTimerRunning = true;
        workoutStartTime = DateTime.Now;
        workoutSession.Start(PlanName);
        _ = RunElapsedTimerAsync();
        ShowNotification("Allenamento iniziato! Forza!");
    }

    [RelayCommand]
    private void MinimizeWorkout()
    {
        workoutSession.Minimize();
    }

    [RelayCommand]
    private void AddExercise()
    {
        Exercises.Add(new WorkoutExercise
        {
            ExerciseName = $"Esercizio {Exercises.Count + 1}",
            BodyPart = "Seleziona",
            Order = Exercises.Count + 1
        });
    }

    [RelayCommand]
    private void RemoveExercise(WorkoutExercise ex)
    {
        Exercises.Remove(ex);
        for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1;
    }

    [RelayCommand]
    private void AddSet(WorkoutExercise ex)
    {
        var last = ex.Sets.LastOrDefault();
        ex.Sets.Add(new ExerciseSet { SetNumber = ex.Sets.Count + 1, WeightKg = last?.WeightKg ?? 20, Reps = last?.Reps ?? 10 });
    }

    [RelayCommand]
    private void RemoveSet(ExerciseSet set)
    {
        var parent = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        parent?.Sets.Remove(set);
        for (int i = 0; i < parent?.Sets.Count; i++) parent.Sets[i].SetNumber = i + 1;
    }

    [RelayCommand] private void IncReps(ExerciseSet s) { s.Reps++; OnPropertyChanged(nameof(s)); }
    [RelayCommand] private void DecReps(ExerciseSet s) { if (s.Reps > 1) s.Reps--; OnPropertyChanged(nameof(s)); }
    [RelayCommand] private void IncWeight(ExerciseSet s) { s.WeightKg += 2.5; OnPropertyChanged(nameof(s)); }
    [RelayCommand] private void DecWeight(ExerciseSet s) { if (s.WeightKg >= 2.5) s.WeightKg -= 2.5; OnPropertyChanged(nameof(s)); }

    [RelayCommand]
    private void CompleteSet(ExerciseSet set)
    {
        set.IsCompleted = true;
        TotalSetsCompleted++;
        StartRestTimer();
    }

    [RelayCommand]
    private void MoveUp(WorkoutExercise ex)
    {
        var i = Exercises.IndexOf(ex);
        if (i > 0) { Exercises.Move(i, i - 1); Renumber(); }
    }

    [RelayCommand]
    private void MoveDown(WorkoutExercise ex)
    {
        var i = Exercises.IndexOf(ex);
        if (i < Exercises.Count - 1) { Exercises.Move(i, i + 1); Renumber(); }
    }

    private void Renumber() { for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1; }

    [RelayCommand]
    private void ReplaceExercise(WorkoutExercise ex) => ShowNotification($"Sostituisci {ex.ExerciseName} — seleziona dal catalogo");

    [RelayCommand]
    private void StartExerciseRestTimer(WorkoutExercise ex)
    {
        restCts?.Cancel();
        restCts = new CancellationTokenSource();
        var token = restCts.Token;
        restSecondsRemaining = RestDuration;
        IsRestTimerActive = true;
        RestTimerLabel = $"pausa {ex.ExerciseName}";
        RestTimerProgress = 1.0;
        RefreshRestTimerUI();
        workoutSession.UpdateRestTimer(true, RestTimerText);
        RunTimerAsync(token);
    }

    private void StartRestTimer()
    {
        restCts?.Cancel();
        restCts = new CancellationTokenSource();
        var token = restCts.Token;
        restSecondsRemaining = RestDuration;
        IsRestTimerActive = true;
        RestTimerLabel = "pausa automatica";
        RestTimerProgress = 1.0;
        RefreshRestTimerUI();
        workoutSession.UpdateRestTimer(true, RestTimerText);
        RunTimerAsync(token);
    }

    private void RunTimerAsync(CancellationToken token)
    {
        Task.Run(async () =>
        {
            try
            {
                while (restSecondsRemaining > 0 && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    restSecondsRemaining--;
                    RestTimerProgress = (double)restSecondsRemaining / RestDuration;
                    RefreshRestTimerUI();
                    workoutSession.UpdateRestTimer(true, RestTimerText);
                    if (restSecondsRemaining is <= 5 and > 0)
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                }
                if (!token.IsCancellationRequested)
                {
                    IsRestTimerActive = false;
                    workoutSession.UpdateRestTimer(false, "");
                    ShowNotification("⏰ Pausa finita! Tempo di spingere!");
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                }
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    private void RefreshRestTimerUI() => RestTimerText = $"{restSecondsRemaining}s";

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
        if (Exercises.Count == 0)
        {
            ShowNotification("Aggiungi almeno un esercizio prima di salvare.");
            return;
        }

        var plan = new WorkoutPlan
        {
            Name = PlanName,
            RestSeconds = RestDuration,
            Exercises = Exercises.ToList(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        PlanStore.SavePlan(plan);

        ShowNotification($"Scheda \"{PlanName}\" salvata!");
        workoutSession.End();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task FinishWorkoutAsync()
    {
        elapsedCts?.Cancel();
        ShowNotification($"Allenamento completato! {TotalSetsCompleted} serie, {Exercises.Count} esercizi.");
        workoutSession.End();
        await Shell.Current.GoToAsync("..");
    }

    private void ShowNotification(string message) => NotificationMessage = message;

    private async Task RunElapsedTimerAsync()
    {
        elapsedCts?.Cancel();
        elapsedCts = new CancellationTokenSource();
        var token = elapsedCts.Token;
        try
        {
            while (!token.IsCancellationRequested)
            {
                var e = DateTime.Now - workoutStartTime;
                ElapsedTime = $"{(int)e.TotalHours:D2}:{e.Minutes:D2}:{e.Seconds:D2}";
                workoutSession.UpdateElapsed(ElapsedTime);
                await Task.Delay(1000, token);
            }
        }
        catch (TaskCanceledException) { }
    }
}
