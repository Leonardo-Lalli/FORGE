using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GymTracker.Mobile.Messages;
using GymTracker.Mobile.Models;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(PlanId), "planId")]
public partial class ActiveWorkoutViewModel : BaseViewModel
{
    private readonly WorkoutSession workoutSession;
    private readonly ExerciseApiService exerciseApi;
    private readonly WgerExerciseService wgerApi;
    private readonly PocketBaseService pb;
    private readonly PlanService planService;
    private readonly DatabaseService db;
    private readonly ConnectivityService connectivity;
    private CancellationTokenSource? restCts;
    private CancellationTokenSource? elapsedCts;
    private int restSecondsRemaining;
    private DateTime workoutStartTime;

    [ObservableProperty] private string mode = "free";
    [ObservableProperty] private string planId = string.Empty;
    [ObservableProperty] private string planName = "Nuovo Allenamento";
    [ObservableProperty] private ObservableCollection<WorkoutExercise> exercises = new();
    [ObservableProperty] private bool hasExercises;
    [ObservableProperty] private int restDuration = 90;

    partial void OnRestDurationChanged(int value)
    {
        if (value < 5) restDuration = 5;
        if (value > 600) restDuration = 600;
        RestTimerLabel = $"pausa {value}s";
    }
    [ObservableProperty] private bool isRestTimerActive;
    [ObservableProperty] private string restTimerText = "90s";
    [ObservableProperty] private double restTimerProgress = 1.0;
    [ObservableProperty] private string elapsedTime = "00:00:00";
    [ObservableProperty] private int totalSetsCompleted;
    [ObservableProperty] private string notificationMessage = string.Empty;
    [ObservableProperty] private string restTimerLabel = "pausa";
    [ObservableProperty] private bool isNotificationVisible;
    [ObservableProperty] private bool isTimerRunning;
    [ObservableProperty] private bool isCreating;

    [ObservableProperty] private bool isSearchVisible;
    [ObservableProperty] private string searchQuery = string.Empty;
    [ObservableProperty] private bool isSearchingApi;
    [ObservableProperty] private string searchError = string.Empty;
    [ObservableProperty] private ObservableCollection<ExerciseSearchResult> searchResults = new();
    private CancellationTokenSource? searchCts;

    [ObservableProperty] private WorkoutExercise? selectedExercise;
    [ObservableProperty] private bool isExerciseDetailVisible;
    [ObservableProperty] private string planNameInput = string.Empty;
    [ObservableProperty] private double progressPercent = 37.5;
    [ObservableProperty] private string workoutNotes = string.Empty;
    [ObservableProperty] private ObservableCollection<FilterChip> muscleFilters = new();
    [ObservableProperty] private ObservableCollection<FilterChip> equipmentFilters = new();

    partial void OnSearchQueryChanged(string value)
    {
        searchCts?.Cancel();
        if (value.Length >= 2)
        {
            searchCts = new CancellationTokenSource();
            var token = searchCts.Token;
            _ = Task.Run(async () =>
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                    MainThread.BeginInvokeOnMainThread(() => _ = SearchExercisesAsync());
            }, token).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                    System.Diagnostics.Debug.WriteLine($"[ActiveWk SearcDeb] ex: {t.Exception.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public ActiveWorkoutViewModel(WorkoutSession workoutSession, ExerciseApiService exerciseApi, WgerExerciseService wgerApi, PocketBaseService pb, PlanService planService, DatabaseService db, ConnectivityService connectivity)
    {
        this.workoutSession = workoutSession;
        this.exerciseApi = exerciseApi;
        this.wgerApi = wgerApi;
        this.pb = pb;
        this.planService = planService;
        this.db = db;
        this.connectivity = connectivity;

        workoutStartTime = DateTime.Now;

        Exercises.CollectionChanged += (_, _) => HasExercises = Exercises.Count > 0;

        MuscleFilters = new ObservableCollection<FilterChip>
        {
            new() { Label = "Tutti", Value = "all", IsSelected = true },
            new() { Label = "Petto", Value = "chest" },
            new() { Label = "Schiena", Value = "back" },
            new() { Label = "Spalle", Value = "shoulders" },
            new() { Label = "Bicipiti", Value = "biceps" },
            new() { Label = "Tricipiti", Value = "triceps" },
            new() { Label = "Addominali", Value = "abdominals" },
            new() { Label = "Quadricipiti", Value = "quadriceps" },
            new() { Label = "Femorali", Value = "hamstrings" },
            new() { Label = "Glutei", Value = "glutes" }
        };

        EquipmentFilters = new ObservableCollection<FilterChip>
        {
            new() { Label = "Tutti", Value = "all", IsSelected = true },
            new() { Label = "Bilanciere", Value = "barbell" },
            new() { Label = "Manubri", Value = "dumbbell" },
            new() { Label = "Cavi", Value = "cable" },
            new() { Label = "Macchinari", Value = "machine" },
            new() { Label = "Corpo libero", Value = "body only" },
            new() { Label = "Kettlebell", Value = "kettlebells" }
        };
    }

    partial void OnModeChanged(string value)
    {
        PlanName = value switch { "free" => "Allenamento Libero", "create" => "Nuova Scheda", _ => "Scheda Salvata" };
        IsCreating = (value == "create");

        HasData = true;
        workoutStartTime = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(PlanId))
            _ = LoadPlanAsync().ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[ActiveWk LoadPlan] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);

        if (!IsCreating)
        {
            IsTimerRunning = true;
            workoutSession.Start(PlanName);
            _ = RunElapsedTimerAsync().ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[ActiveWk Elapsed] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    partial void OnPlanIdChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && Mode == "plan")
            _ = LoadPlanAsync().ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[ActiveWk LoadPlan2] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private async Task LoadPlanAsync()
    {
        var plans = await planService.LoadPlansAsync();
        var plan = plans.FirstOrDefault(p => p.Id == PlanId);
        if (plan == null) return;

        PlanName = plan.Name;
        PlanNameInput = plan.Name;
        RestDuration = plan.RestSeconds;

        Exercises.Clear();
        foreach (var ex in plan.Exercises)
        {
            ex.Order = Exercises.Count + 1;
            // Clone sets to avoid modifying the saved plan
            var clonedSets = new ObservableCollection<ExerciseSet>();
            foreach (var s in ex.Sets)
                clonedSets.Add(new ExerciseSet { SetNumber = s.SetNumber, WeightKg = s.WeightKg, Reps = s.Reps, IsCompleted = false });
            ex.Sets = clonedSets;
            ex.IsCompleted = false;
            Exercises.Add(ex);
        }
    }

    partial void OnNotificationMessageChanged(string value)
    {
        IsNotificationVisible = !string.IsNullOrWhiteSpace(value);
    }

    [RelayCommand]
    private void StartWorkout()
    {
        if (!string.IsNullOrWhiteSpace(PlanNameInput))
            PlanName = PlanNameInput;
        IsCreating = false;
        IsTimerRunning = true;
        workoutStartTime = DateTime.Now;
        workoutSession.Start(PlanName);
        _ = RunElapsedTimerAsync().ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[ActiveWk Elapsed2] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
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
        IsSearchVisible = true;
        SearchQuery = string.Empty;
        SearchResults.Clear();
        SearchError = string.Empty;
    }

    [RelayCommand]
    private void CloseSearch()
    {
        IsSearchVisible = false;
    }

    [RelayCommand]
    private async Task SelectMuscleFilterAsync(FilterChip chip)
    {
        foreach (var m in MuscleFilters) m.IsSelected = false;
        chip.IsSelected = true;
        SearchError = string.Empty;

        if (chip.Value == "all") { SearchQuery = ""; return; }

        IsSearchingApi = true;
        try
        {
            var results = await exerciseApi.GetByMuscleAsync(chip.Value);
            if (results.Count == 0)
                results = await wgerApi.GetByMuscleAsync(chip.Value);
            SearchResults.Clear();
            foreach (var r in results.Take(10))
            {
                var img = r.Images.FirstOrDefault() ?? "";
                SearchResults.Add(new ExerciseSearchResult
                {
                    Id = r.Id, Name = r.Name,
                    BodyPart = r.PrimaryMuscles.FirstOrDefault() ?? "",
                    Equipment = r.Equipment,
                    ImageUrl = img.StartsWith("http") ? img : $"https://{img}"
                });
            }
        }
        catch { SearchError = "API non raggiungibile."; }
        finally { IsSearchingApi = false; }
    }

    [RelayCommand]
    private void SelectEquipmentFilter(FilterChip chip)
    {
        foreach (var e in EquipmentFilters) e.IsSelected = false;
        chip.IsSelected = true;
        // Filter client-side by equipment
        SearchError = "";
    }

    [RelayCommand]
    private async Task SearchExercisesAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2)
        {
            SearchError = "Digita almeno 2 caratteri.";
            return;
        }

        SearchError = string.Empty;
        IsSearchingApi = true;
        try
        {
            var results = await exerciseApi.SearchByNameAsync(SearchQuery);
            if (results.Count == 0)
            {
                try { results = await wgerApi.SearchExercisesAsync(SearchQuery, language: "2"); }
                catch { }
            }

            SearchResults.Clear();
            if (results.Count == 0)
            {
                SearchError = "Nessun esercizio trovato. Prova un altro nome.";
            }
            else
            {
                foreach (var r in results.Take(10))
                {
                    var img = r.Images.FirstOrDefault() ?? "";

                    var wgerImg = await wgerApi.GetImageForExerciseAsync(r.Name);
                    if (!string.IsNullOrWhiteSpace(wgerImg))
                        img = wgerImg;
                    else if (!img.StartsWith("http") || img.Contains("encr.pw") || img.Contains("acesse.dev"))
                        img = "";

                    SearchResults.Add(new ExerciseSearchResult
                    {
                        Id = r.Id,
                        Name = r.Name,
                        BodyPart = r.PrimaryMuscles.FirstOrDefault() ?? string.Empty,
                        Equipment = r.Equipment,
                        ImageUrl = img
                    });
                }
            }
        }
        catch (HttpRequestException)
        {
            SearchError = "API non raggiungibile. Controlla la connessione.";
            SearchResults.Clear();
        }
        catch (Exception ex)
        {
            SearchError = $"Errore: {ex.Message}";
            SearchResults.Clear();
        }
        finally
        {
            IsSearchingApi = false;
        }
    }

    [RelayCommand]
    private async Task SelectExerciseAsync(ExerciseSearchResult result)
    {
        var full = await exerciseApi.GetExerciseByIdAsync(result.Id);
        var imageUrl = "";
        var gifUrl = "";
        var instructions = new List<string>();

        if (full != null)
        {
            if (full.Images.Count > 0)
            {
                var img = full.Images[0];
                imageUrl = img.StartsWith("http") ? img : $"https://{img}";
                System.Diagnostics.Debug.WriteLine($"[SelectExercise] raw={img} resolved={imageUrl}");
            }
            gifUrl = imageUrl;
            instructions = full.Instructions;
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
            imageUrl = result.ImageUrl;

        // Try PocketBase cache for resolved image
        var cached = await exerciseApi.GetCachedImageUrlAsync(result.Name);
        if (!string.IsNullOrWhiteSpace(cached) && cached.StartsWith("http"))
        {
            imageUrl = cached;
            System.Diagnostics.Debug.WriteLine($"[SelectExercise] using cached image: {cached[..Math.Min(cached.Length, 80)]}");
        }

        System.Diagnostics.Debug.WriteLine($"[SelectExercise] final ImageUrl={imageUrl}");

        Exercises.Add(new WorkoutExercise
        {
            ExerciseId = result.Id,
            ExerciseName = result.Name,
            BodyPart = result.BodyPart,
            Equipment = result.Equipment,
            ImageUrl = imageUrl,
            GifUrl = gifUrl,
            Instructions = instructions,
            Order = Exercises.Count + 1
        });
        IsSearchVisible = false;
        SearchQuery = string.Empty;
        SearchResults.Clear();
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
    private void ReplaceExercise(WorkoutExercise ex)
    {
        Exercises.Remove(ex);
        for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1;
        AddExercise();
    }

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
            catch (TaskCanceledException) { /* timer cancelled, expected */ }
        }, token).ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
                System.Diagnostics.Debug.WriteLine($"[ActiveWk RunTimer] ex: {t.Exception.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
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
    private void OpenExerciseDetail(WorkoutExercise ex)
    {
        SelectedExercise = ex;
        IsExerciseDetailVisible = true;
    }

    [RelayCommand]
    private void CloseExerciseDetail()
    {
        IsExerciseDetailVisible = false;
        SelectedExercise = null;
    }

    [RelayCommand]
    private async Task SaveWorkoutPlanAsync()
    {
        var name = string.IsNullOrWhiteSpace(PlanNameInput) ? PlanName : PlanNameInput;
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowNotification("Dai un nome alla scheda prima di salvarla.");
            return;
        }
        if (Exercises.Count == 0)
        {
            ShowNotification("Aggiungi almeno un esercizio prima di salvare.");
            return;
        }

        PlanName = name;

        var plan = new WorkoutPlan
        {
            Name = name,
            RestSeconds = RestDuration,
            Exercises = Exercises.ToList(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await planService.SavePlanAsync(plan);

        await SaveToPocketBaseAsync(name);

        ShowNotification($"Scheda \"{name}\" salvata!");
        workoutSession.End();
        await Shell.Current.GoToAsync("..");
    }

    private async Task SaveToPocketBaseAsync(string name)
    {
        if (!pb.IsLoggedIn) return;

        try
        {
            var exerciseData = Exercises.Select(e => new
            {
                name = e.ExerciseName,
                bodyPart = e.BodyPart,
                equipment = e.Equipment,
                notes = e.Notes,
                restSeconds = e.RestSeconds,
                sets = e.Sets.Select(s => new
                {
                    setNumber = s.SetNumber,
                    weightKg = s.WeightKg,
                    reps = s.Reps,
                    isCompleted = s.IsCompleted
                }).ToList()
            }).ToList();

            var volume = Exercises.Sum(e => e.Sets.Sum(s => s.WeightKg * s.Reps));
            var duration = (int)(DateTime.Now - workoutStartTime).TotalMinutes;

            var payload = new Dictionary<string, object>
            {
                ["user"] = pb.CurrentUser!.Id,
                ["user_name"] = pb.CurrentUser.Name,
                ["name"] = name,
                ["date"] = DateTime.UtcNow.ToString("o"),
                ["notes"] = WorkoutNotes ?? "",
                ["exercises"] = Exercises.Select(e => e.ExerciseName).ToList(),
                ["exercise_data"] = System.Text.Json.JsonSerializer.Serialize(exerciseData),
                ["volume"] = volume,
                ["duration"] = Math.Max(1, duration)
            };
            var (ok, err) = await pb.CreateRecordAsync("logged_workouts", payload);
            System.Diagnostics.Debug.WriteLine($"[SaveWorkout] PB result: ok={ok} err={err}");

            var localWorkout = new Models.LocalWorkout
            {
                Id = Guid.NewGuid().ToString(),
                UserId = pb.CurrentUser!.Id,
                Name = name,
                Date = DateTime.UtcNow.ToString("o"),
                ExercisesJson = System.Text.Json.JsonSerializer.Serialize(Exercises.Select(e => e.ExerciseName).ToList()),
                Volume = volume,
                Duration = Math.Max(1, duration),
                Notes = WorkoutNotes ?? "",
                ExerciseDataJson = System.Text.Json.JsonSerializer.Serialize(exerciseData),
                UserName = pb.CurrentUser.Name,
                PendingSync = !ok
            };
            await db.SaveWorkoutAsync(localWorkout);

            WeakReferenceMessenger.Default.Send(new WorkoutSavedMessage());

            if (!ok)
                ShowNotification($"Errore salvataggio: {err}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SaveWorkout] PB ex: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task FinishWorkoutAsync()
    {
        elapsedCts?.Cancel();
        restCts?.Cancel();

        if (Exercises.Count > 0 && IsTimerRunning)
        {
            await SaveToPocketBaseAsync(PlanName);
        }

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
        catch (TaskCanceledException) { /* elapsed timer cancelled, expected */ }
    }
}

public partial class FilterChip : ObservableObject
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    [ObservableProperty] private bool isSelected;
}

public class ExerciseSearchResult
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
