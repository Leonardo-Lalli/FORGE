using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Models;
using Forge.Services;

namespace Forge.ViewModels;

public partial class ActiveWorkoutViewModel : BaseViewModel, IQueryAttributable
{
    private readonly WorkoutSession workoutSession;
    private readonly ExerciseDbApiService exerciseDbApi;
    private readonly PocketBaseService pb;
    private readonly PlanService planService;
    private readonly DatabaseService db;
    private readonly ConnectivityService connectivity;
    private readonly NotificationService notifications;
    private CancellationTokenSource? restCts;
    private CancellationTokenSource? elapsedCts;
    private int restSecondsRemaining;
    private DateTime workoutStartTime;
    private bool restoringDraft;
    private string? sessionDraftId;
    private bool planNotesReady;
    private string planNotesOwner = "";
    private CancellationTokenSource? notesSaveCts;
    private readonly SemaphoreSlim notesSaveGate = new(1, 1);
    private readonly HashSet<WorkoutExercise> noteSubscriptions = [];
    private int activeRestDuration;
    private List<LocalWorkout> previousHistory = [];
    private WorkoutExercise? replacingExercise;
    [ObservableProperty] private bool isExerciseMenuVisible;
    [ObservableProperty] private bool isRestPickerVisible;
    [ObservableProperty] private int restPickerSeconds = 90;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PauseTimerLabel))] private bool isRestPaused;
    public string PauseTimerLabel => IsRestPaused ? "▶" : "Ⅱ";
    [RelayCommand] private void ShowExerciseMenu(WorkoutExercise ex) { SelectedExercise = ex; IsExerciseMenuVisible = true; }
    [RelayCommand] private void CloseExerciseMenu() => IsExerciseMenuVisible = false;
    [RelayCommand] private void MenuMoveUp() { if (SelectedExercise != null) MoveUp(SelectedExercise); IsExerciseMenuVisible = false; }
    [RelayCommand] private void MenuMoveDown() { if (SelectedExercise != null) MoveDown(SelectedExercise); IsExerciseMenuVisible = false; }
    [RelayCommand] private void MenuRemove() { if (SelectedExercise != null) RemoveExercise(SelectedExercise); IsExerciseMenuVisible = false; }
    [RelayCommand] private async Task MenuReplaceAsync() { IsExerciseMenuVisible = false; if (SelectedExercise != null) await ReplaceExerciseAsync(SelectedExercise); }
    [RelayCommand] private void ShowRestPicker(WorkoutExercise ex) { SelectedExercise = ex; RestPickerSeconds = ex.RestSeconds; IsRestPickerVisible = true; }
    [RelayCommand] private void CloseRestPicker() => IsRestPickerVisible = false;
    [RelayCommand] private async Task SaveRestPickerAsync()
    {
        if (SelectedExercise == null) return;
        SelectedExercise.RestSeconds = RestPickerSeconds;
        IsRestPickerVisible = false;
    }
    [RelayCommand] private async Task StartPickedRestAsync()
    {
        await SaveRestPickerAsync();
        await notifications.EnsureRestPermissionAsync();
        if (SelectedExercise != null && !IsCreating) StartExerciseRestTimer(SelectedExercise);
    }
    [RelayCommand] private void PauseRestTimer() { notifications.ToggleRestPause(); IsRestPaused = notifications.IsRestPaused; }
    [RelayCommand] private void RestSoundSettings() => notifications.OpenRestSoundSettings();
    [RelayCommand] private void EnableExactRest() => notifications.RequestExactRestPermission();
    public IReadOnlyList<string> ProgressMetrics { get; } = new[] { "Peso massimo (kg)", "Serie completate", "Ripetizioni totali", "Volume totale (kg)" };
    [ObservableProperty, NotifyPropertyChangedFor(nameof(SelectedProgressMetric))] private int progressMetricIndex;
    public string SelectedProgressMetric => ProgressMetrics[Math.Clamp(ProgressMetricIndex, 0, ProgressMetrics.Count - 1)];
    [ObservableProperty] private bool isProgressMetricMenuVisible;
    [RelayCommand] private void ShowProgressMetricMenu() => IsProgressMetricMenuVisible = true;
    [RelayCommand] private void CloseProgressMetricMenu() => IsProgressMetricMenuVisible = false;
    [RelayCommand] private void SelectProgressMetric(string value) { ProgressMetricIndex = int.Parse(value); IsProgressMetricMenuVisible = false; }
    [ObservableProperty] private ObservableCollection<ExerciseProgressPoint> exerciseProgressPoints = new();
    [ObservableProperty] private bool hasExerciseProgress;
    partial void OnProgressMetricIndexChanged(int value) => BuildExerciseProgress();
    private void BuildExerciseProgress()
    {
        ExerciseProgressPoints = new(SelectedExercise == null ? [] : ExerciseProgress.Build(previousHistory, SelectedExercise, ProgressMetricIndex));
        HasExerciseProgress = ExerciseProgressPoints.Count > 0;
    }
    public IReadOnlyList<int> RestMinuteOptions { get; } = Enumerable.Range(0, 6).ToList();
    public IReadOnlyList<int> RestSecondOptions { get; } = Enumerable.Range(0, 60).ToList();

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsPlanWorkout))] private string mode = "";
    public bool IsPlanWorkout => Mode == "plan" && !string.IsNullOrWhiteSpace(PlanId);
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsPlanWorkout))] private string planId = string.Empty;
    [ObservableProperty] private string planName = "Nuovo Allenamento";
    [ObservableProperty] private ObservableCollection<WorkoutExercise> exercises = new();
    [ObservableProperty] private bool hasExercises;
    [ObservableProperty] private int restDuration = 90;

    partial void OnRestDurationChanged(int value)
    {
        if (value < 0) restDuration = 0;
        if (value > 300) restDuration = 300;
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
    public bool HasSearchError => !string.IsNullOrWhiteSpace(SearchError);

    partial void OnSearchErrorChanged(string value) => OnPropertyChanged(nameof(HasSearchError));
    [ObservableProperty] private ObservableCollection<ExerciseSearchResult> searchResults = new();
    private CancellationTokenSource? searchCts;
    private int searchGeneration;

    [ObservableProperty] private WorkoutExercise? selectedExercise;
    [ObservableProperty] private bool isExerciseDetailVisible;
    [ObservableProperty] private string planNameInput = string.Empty;
    [ObservableProperty] private double progressPercent = 37.5;
    [ObservableProperty] private string workoutNotes = string.Empty;
    partial void OnWorkoutNotesChanged(string value) => QueueNotesSave();

    private void TrackExerciseNotes()
    {
        foreach (var exercise in noteSubscriptions.Where(e => !Exercises.Contains(e)).ToList())
        {
            exercise.PropertyChanged -= OnExerciseNoteChanged;
            noteSubscriptions.Remove(exercise);
        }
        foreach (var exercise in Exercises)
            if (noteSubscriptions.Add(exercise)) exercise.PropertyChanged += OnExerciseNoteChanged;
    }
    private void OnExerciseNoteChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WorkoutExercise.Notes)) QueueNotesSave();
    }
    private void QueueNotesSave()
    {
        if (!planNotesReady || restoringDraft || Mode != "plan" || IsCreating) return;
        notesSaveCts?.Cancel();
        notesSaveCts?.Dispose();
        notesSaveCts = new();
        _ = SaveNotesAfterTypingAsync(notesSaveCts.Token);
    }
    private async Task SaveNotesAfterTypingAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(400, token);
            await PersistPlanNotesAsync();
        }
        catch (OperationCanceledException) { }
    }
    private async Task<bool> PersistPlanNotesAsync()
    {
        if (!planNotesReady || Mode != "plan" || IsCreating) return true;
        notesSaveCts?.Cancel();
        await notesSaveGate.WaitAsync();
        try
        {
            var notes = Exercises.Select(e => new PlanSessionChanges.ExerciseNote(e.Id, e.ExerciseId, e.ExerciseName, e.Notes)).ToList();
            await planService.SaveNotesAsync(PlanId, planNotesOwner, WorkoutNotes, notes);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlanNotes] {ex.GetType().Name}");
            ShowNotification("Impossibile salvare le note della scheda. Riprova prima di uscire.");
            return false;
        }
        finally { notesSaveGate.Release(); }
    }
    [ObservableProperty] private ObservableCollection<FilterChip> muscleFilters = new();
    [ObservableProperty] private ObservableCollection<FilterChip> equipmentFilters = new();
    [ObservableProperty] private ObservableCollection<string> workoutPhotos = new();
    [ObservableProperty] private bool hasWorkoutPhotos;

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        if (WorkoutPhotos.Count >= 5)
        {
            ShowNotification("Massimo 5 foto per allenamento.");
            return;
        }
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                ShowNotification("Fotocamera non disponibile.");
                return;
            }
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;
            await AddPhotoFromFileAsync(photo);
        }
        catch (PermissionException)
        {
            ShowNotification("Permesso fotocamera negato.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TakePhoto] ex: {ex.Message}");
            ShowNotification("Errore nell'acquisizione foto.");
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        if (WorkoutPhotos.Count >= 5)
        {
            ShowNotification("Massimo 5 foto per allenamento.");
            return;
        }
        try
        {
            var photos = await MediaPicker.Default.PickPhotosAsync();
            if (photos == null) return;
            foreach (var photo in photos)
            {
                if (WorkoutPhotos.Count >= 5) break;
                await AddPhotoFromFileAsync(photo);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PickPhoto] ex: {ex.Message}");
            ShowNotification("Errore nella selezione foto.");
        }
    }

    private async Task AddPhotoFromFileAsync(FileResult file)
    {
        var dataUri = await WorkoutPhotoService.PrepareAsync(file);
        WorkoutPhotos.Add(dataUri);
        HasWorkoutPhotos = WorkoutPhotos.Count > 0;
    }

    [RelayCommand]
    private void RemovePhoto(string photo)
    {
        WorkoutPhotos.Remove(photo);
        HasWorkoutPhotos = WorkoutPhotos.Count > 0;
    }

    partial void OnSearchQueryChanged(string value)
    {
        searchCts?.Cancel();
        searchGeneration++;
        if (value.Length < 2 && IsSearchVisible) { _ = ApplyFiltersAsync(); return; }
        if (value.Length >= 2)
        {
            searchCts = new CancellationTokenSource();
            var token = searchCts.Token;
            _ = Task.Run(async () =>
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                    MainThread.BeginInvokeOnMainThread(() => _ = SearchExercisesWithCancellationAsync(token));
            }, token).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                    System.Diagnostics.Debug.WriteLine($"[ActiveWk SearcDeb] ex: {t.Exception.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public ActiveWorkoutViewModel(WorkoutSession workoutSession, ExerciseDbApiService exerciseDbApi, PocketBaseService pb, PlanService planService, DatabaseService db, ConnectivityService connectivity, NotificationService notifications)
    {
        this.workoutSession = workoutSession;
        this.exerciseDbApi = exerciseDbApi;
        this.pb = pb;
        this.planService = planService;
        this.notifications = notifications;
        this.db = db;
        this.connectivity = connectivity;

        _ = exerciseDbApi.WarmCacheAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                System.Diagnostics.Debug.WriteLine($"[ActiveWk WarmCache] ex: {t.Exception?.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);

        workoutStartTime = DateTime.Now;

        Exercises.CollectionChanged += (_, _) => { HasExercises = Exercises.Count > 0; TrackExerciseNotes(); };

        MuscleFilters = new ObservableCollection<FilterChip>
        {
            new() { Label = "Tutti", Value = "", IsSelected = true },
            new() { Label = "Petto", Value = "chest,pectorals,pectoralis major,pectoralis minor,serratus anterior" },
            new() { Label = "Schiena", Value = "back,lats,latissimus dorsi,upper back,lower back,spine,erector spinae,traps,trapezius,rhomboids,infraspinatus,teres major,teres minor" },
            new() { Label = "Spalle", Value = "shoulders,delts,deltoids,front delt,side delt,rear delt,anterior deltoid,lateral deltoid,posterior deltoid" },
            new() { Label = "Bicipiti", Value = "biceps,brachialis" },
            new() { Label = "Tricipiti", Value = "triceps" },
            new() { Label = "Gambe", Value = "upper legs,lower legs,glutes,hamstrings,quads,quadriceps,calves,abductors,hip flexors,adductors" },
            new() { Label = "Addominali", Value = "waist,abs,core,obliques,rectus abdominis" },
            new() { Label = "Cardio", Value = "cardio,cardiovascular system" }
        };

        EquipmentFilters = new ObservableCollection<FilterChip>
        {
            new() { Label = "Tutti", Value = "", IsSelected = true },
            new() { Label = "Corpo libero", Value = "body weight,bodyweight,body-weight" },
            new() { Label = "Manubri", Value = "dumbbell" },
            new() { Label = "Bilanciere", Value = "barbell" },
            new() { Label = "Cavi", Value = "cable" },
            new() { Label = "Macchinari", Value = "machine,leverage,assisted" },
            new() { Label = "Elastico", Value = "band" }
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("mode", out var modeValue)) return;
        var requestedMode = modeValue?.ToString() ?? "free";
        var requestedPlanId = query.TryGetValue("planId", out var id) ? id?.ToString() ?? "" : "";
        _ = InitializeSessionAsync(requestedMode, requestedPlanId);
    }

    private async Task InitializeSessionAsync(string requestedMode, string requestedPlanId)
    {
        if (IsBusy || IsTimerRunning || IsCreating) return;
        IsBusy = true;
        ErrorMessage = null;
        HasData = false;
        IsEmptyState = false;
        planNotesReady = false;
        notesSaveCts?.Cancel();
        try
        {
            // Apply navigation parameters together. Quick Start never adopts a saved plan or draft.
            Mode = requestedMode is "plan" or "create" or "resume" ? requestedMode : "free";
            PlanId = Mode == "plan" ? requestedPlanId : "";
            if (Mode == "plan" && string.IsNullOrWhiteSpace(PlanId)) Mode = "free";
            IsCreating = Mode == "create";
            Exercises.Clear();
            WorkoutPhotos.Clear();
            HasWorkoutPhotos = false;
            WorkoutNotes = "";
            PlanNameInput = "";
            PlanName = IsCreating ? "Nuova Scheda" : "Allenamento Libero";
            originalPlanSignature = null;
            sessionDraftId = null;
            planChoiceResolved = false;
            updatePlanOnFinish = false;
            IsPlanUpdateVisible = false;
            workoutStartTime = DateTime.Now;

            if (Mode != "resume") notifications.CancelRest();
            if (Mode == "plan") await LoadPlanAsync();
            else if (Mode == "resume") await LoadDraftAsync();

            HasData = true;
            if (!IsCreating)
            {
                IsTimerRunning = true;
                workoutSession.Start(PlanName);
                _ = RunElapsedTimerAsync();
                if (notifications.RestRemaining > 0) { IsRestTimerActive = true; activeRestDuration = notifications.RestRemaining; restCts = new(); RunTimerAsync(restCts.Token); }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ActiveWk Initialize] {ex.GetType().Name}");
            HasData = false;
            ErrorMessage = "Impossibile aprire la sessione. Torna indietro e riprova.";
            ShowNotification("Impossibile aprire la sessione. Torna indietro e riprova.");
        }
        finally { IsBusy = false; }
    }

    private string? originalPlanSignature;
    private bool planChoiceResolved;
    private bool updatePlanOnFinish;
    [ObservableProperty] private bool isPlanUpdateVisible;
    [ObservableProperty] private bool isFinishingWorkout;
    [ObservableProperty] private bool isSetKindVisible;
    private ExerciseSet? selectedSet;
    [RelayCommand] private void ShowSetKind(ExerciseSet set) { selectedSet = set; IsSetKindVisible = true; }
    [RelayCommand] private void CloseSetKind() { selectedSet = null; IsSetKindVisible = false; }
    [RelayCommand] private void SelectSetKind(string kind) { if (selectedSet != null && kind is "normal" or "warmup" or "drop" or "failure") selectedSet.Kind = kind; CloseSetKind(); }
    [RelayCommand] private void CancelPlanUpdate() { IsPlanUpdateVisible = false; }
    [RelayCommand] private async Task UpdatePlanAndFinishAsync() { IsPlanUpdateVisible = false; updatePlanOnFinish = true; planChoiceResolved = true; await FinishWorkoutAsync(); }
    [RelayCommand] private async Task KeepPlanAndFinishAsync() { IsPlanUpdateVisible = false; updatePlanOnFinish = false; planChoiceResolved = true; await FinishWorkoutAsync(); }
    private string CurrentPlanSignature() => PlanSessionChanges.Signature(string.IsNullOrWhiteSpace(PlanNameInput) ? PlanName : PlanNameInput, WorkoutNotes, RestDuration, Exercises);

    private async Task LoadPlanAsync()
    {
        planNotesReady = false;
        notesSaveCts?.Cancel();
        var plans = await planService.LoadPlansAsync();
        var plan = plans.FirstOrDefault(p => p.Id == PlanId);
        if (plan == null) throw new InvalidOperationException("Scheda non disponibile.");

        PlanName = plan.Name;
        PlanNameInput = plan.Name;
        WorkoutNotes = plan.Notes;
        previousHistory = await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "");
        RestDuration = plan.RestSeconds;

        Exercises.Clear();
        foreach (var ex in plan.Exercises)
        {
            ex.Order = Exercises.Count + 1;
            // Clone sets to avoid modifying the saved plan
            var clonedSets = new ObservableCollection<ExerciseSet>();
            foreach (var s in ex.Sets)
                clonedSets.Add(new ExerciseSet { SetNumber = s.SetNumber, WeightKg = s.WeightKg, Reps = s.Reps, Kind = s.Kind, IsCompleted = false });
            ex.Sets = clonedSets;
            ex.IsCompleted = false;
            PreviousPerformance.Apply(previousHistory, ex);
            await EnrichExerciseAsync(ex);
            Exercises.Add(ex);
        }
        originalPlanSignature = PlanSessionChanges.Signature(plan.Name, plan.Notes, plan.RestSeconds, plan.Exercises);
        planNotesOwner = plan.UserId;
        planNotesReady = true;
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
    private void CollapseWorkout()
    {
        workoutSession.Minimize();
    }

    [RelayCommand]
    private async Task AddExerciseAsync()
    {
        replacingExercise = null;
        await ShowSearchAsync();
    }
    private async Task ShowSearchAsync()
    {
        IsSearchVisible = true;
        SearchQuery = string.Empty;
        SearchResults.Clear();
        SearchError = string.Empty;
        await ApplyFiltersAsync();
    }

    [RelayCommand]
    private void CloseSearch()
    {
        searchCts?.Cancel();
        replacingExercise = null;
        searchGeneration++;
        IsSearchVisible = false;
    }

    [RelayCommand]
    private async Task SelectMuscleFilterAsync(FilterChip chip)
    {
        ToggleFilter(MuscleFilters, chip);
        await ApplyFiltersAsync();
    }

    [RelayCommand]
    private async Task SelectEquipmentFilterAsync(FilterChip chip)
    {
        ToggleFilter(EquipmentFilters, chip);
        await ApplyFiltersAsync();
    }

    private async Task ApplyFiltersAsync()
    {
        var generation = ++searchGeneration;
        var query = SearchQuery;
        var activeMuscles = MuscleFilters.Where(f => f.IsSelected && !string.IsNullOrWhiteSpace(f.Value)).ToList();
        var activeEquip = EquipmentFilters.Where(f => f.IsSelected && !string.IsNullOrWhiteSpace(f.Value)).ToList();

        IsSearchingApi = true;
        try
        {
            await exerciseDbApi.WarmCacheAsync();
            var all = await exerciseDbApi.GetCatalogAsync();
            if (generation != searchGeneration || !IsSearchVisible) return;
            if (all.Count == 0) { SearchError = "Catalogo non disponibile. Controlla la connessione e riprova."; return; }

            var results = all.AsEnumerable();

            if (activeMuscles.Count > 0)
            {
                results = results.Where(e =>
                {
                    return activeMuscles.Any(chip => ExerciseMuscleFilter.Matches(e.BodyPart, e.TargetMuscles, e.Category, chip.GetValues()));
                });
            }

            if (activeEquip.Count > 0)
            {
                results = results.Where(e =>
                    activeEquip.Any(chip =>
                    {
                        var vals = chip.GetValues();
                        return vals.Any(v =>
                            e.Equipment.Contains(v, StringComparison.OrdinalIgnoreCase));
                    }));
            }

            if (query.Trim().Length >= 2)
            {
                var matchingIds = (await exerciseDbApi.SearchAsync(query)).Select(e => e.Id).ToHashSet();
                results = results.Where(e => matchingIds.Contains(e.Id));
            }
            if (generation != searchGeneration || !IsSearchVisible) return;
            var final = results.ToList();

            SearchResults = new ObservableCollection<ExerciseSearchResult>(final.Select(r => new ExerciseSearchResult
            {
                Id = r.Id, Name = r.Name, BodyPart = r.BodyPart, Equipment = r.Equipment, ImageUrl = r.ImageUrl
            }));
            SearchError = final.Count == 0 ? "Nessun esercizio trovato con questi filtri." : "";
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[ActiveWk Filter] ex: {ex.Message}"); if (generation == searchGeneration) SearchError = "Cache non disponibile."; }
        finally { if (generation == searchGeneration) IsSearchingApi = false; }
    }

    [RelayCommand]
    private async Task SearchExercisesAsync()
    {
        await SearchExercisesWithCancellationAsync(CancellationToken.None);
    }

    private async Task SearchExercisesWithCancellationAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return;
        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2)
        {
            await ApplyFiltersAsync();
            return;
        }
        var generation = ++searchGeneration;
        SearchError = string.Empty;
        IsSearchingApi = true;
        try
        {
            var results = await exerciseDbApi.SearchAsync(SearchQuery);
            if (ct.IsCancellationRequested || generation != searchGeneration || !IsSearchVisible) return;

            SearchResults = new ObservableCollection<ExerciseSearchResult>(results.Where(PassesSelectedFilters).Select(r => new ExerciseSearchResult
            {
                Id = r.Id, Name = r.Name, BodyPart = r.BodyPart, Equipment = r.Equipment, ImageUrl = r.ImageUrl
            }));
            SearchError = SearchResults.Count == 0 ? "Nessun esercizio trovato con questi filtri." : "";
        }
        catch (HttpRequestException)
        {
            if (generation != searchGeneration || ct.IsCancellationRequested) return;
            SearchError = "API non raggiungibile. Controlla la connessione.";
            SearchResults.Clear();
        }
        catch (Exception ex)
        {
            if (generation != searchGeneration || ct.IsCancellationRequested) return;
            SearchError = $"Errore: {ex.Message}";
            SearchResults.Clear();
        }
        finally
        {
            if (generation == searchGeneration) IsSearchingApi = false;
        }
    }

    private static void ToggleFilter(ObservableCollection<FilterChip> filters, FilterChip chip)
    {
        if (string.IsNullOrEmpty(chip.Value))
        { foreach (var item in filters) item.IsSelected = item == chip; return; }
        chip.IsSelected = !chip.IsSelected;
        filters.First(f => string.IsNullOrEmpty(f.Value)).IsSelected = !filters.Any(f => f.IsSelected && !string.IsNullOrEmpty(f.Value));
    }

    private bool PassesSelectedFilters(ExerciseSearchResultDto exercise)
    {
        var muscles = MuscleFilters.Where(f => f.IsSelected && !string.IsNullOrEmpty(f.Value)).ToList();
        var equipment = EquipmentFilters.Where(f => f.IsSelected && !string.IsNullOrEmpty(f.Value)).ToList();
        return (muscles.Count == 0 || muscles.Any(f => ExerciseMuscleFilter.Matches(exercise.BodyPart, exercise.TargetMuscles, exercise.Category, f.GetValues()))) &&
            (equipment.Count == 0 || equipment.Any(f => f.GetValues().Any(v => exercise.Equipment.Contains(v, StringComparison.OrdinalIgnoreCase))));
    }

    [RelayCommand]
    private async Task SelectExerciseAsync(ExerciseSearchResult result)
    {
        var cached = await db.GetCachedExerciseByIdAsync(result.Id);
        previousHistory = await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "");
        var exercise = new WorkoutExercise
        {
            ExerciseId = result.Id,
            ExerciseName = result.Name,
            BodyPart = result.BodyPart,
            Equipment = result.Equipment,
            ImageUrl = result.ImageUrl,
            GifUrl = result.ImageUrl,
            Instructions = cached == null ? [] : System.Text.Json.JsonSerializer.Deserialize<List<string>>(cached.InstructionsJson) ?? [],
            RestSeconds = RestDuration,
            Order = Exercises.Count + 1
        };
        var previous = PreviousPerformance.Find(previousHistory, exercise);
        foreach (var (weight, reps) in previous)
            exercise.Sets.Add(new ExerciseSet { SetNumber = exercise.Sets.Count + 1, WeightKg = weight, Reps = reps, IsSuggested = true, PreviousText = $"{weight:0.#} × {reps}" });
        if (exercise.Sets.Count == 0) exercise.Sets.Add(new ExerciseSet { SetNumber = 1, Reps = 10 });
        var index = replacingExercise == null ? -1 : Exercises.IndexOf(replacingExercise);
        if (index >= 0)
        {
            exercise.RestSeconds = replacingExercise!.RestSeconds;
            exercise.Notes = replacingExercise.Notes;
            Exercises[index] = exercise;
        }
        else Exercises.Add(exercise);
        replacingExercise = null;
        Renumber();
        TotalSetsCompleted = Exercises.Sum(e => e.Sets.Count(s => s.IsCompleted));
        IsSearchVisible = false;
        SearchQuery = string.Empty;
        SearchResults.Clear();
    }

    [RelayCommand]
    private void RemoveExercise(WorkoutExercise ex)
    {
        Exercises.Remove(ex);
        TotalSetsCompleted = Exercises.Sum(e => e.Sets.Count(s => s.IsCompleted));
        for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1;
    }

    [RelayCommand]
    private void AddSet(WorkoutExercise ex)
    {
        var last = ex.Sets.LastOrDefault();
        var previous = PreviousPerformance.Find(previousHistory, ex);
        var suggestion = ex.Sets.Count < previous.Count ? previous[ex.Sets.Count] : ((double Weight, int Reps)?)null;
        ex.Sets.Add(new ExerciseSet { SetNumber = ex.Sets.Count + 1, WeightKg = suggestion?.Weight ?? last?.WeightKg ?? 0, Reps = suggestion?.Reps ?? last?.Reps ?? 10, IsSuggested = suggestion != null, PreviousText = suggestion is { } previousSet ? $"{previousSet.Weight:0.#} × {previousSet.Reps}" : "—" });
    }

    [RelayCommand]
    private void RemoveSet(ExerciseSet set)
    {
        var parent = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        parent?.Sets.Remove(set);
        for (int i = 0; i < parent?.Sets.Count; i++) parent.Sets[i].SetNumber = i + 1;
    }

    [RelayCommand] private void IncReps(ExerciseSet s) => s.Reps++;
    [RelayCommand] private void DecReps(ExerciseSet s) { if (s.Reps > 1) s.Reps--; }
    [RelayCommand] private void IncWeight(ExerciseSet s) => s.WeightKg += 2.5;
    [RelayCommand] private void DecWeight(ExerciseSet s) { if (s.WeightKg >= 2.5) s.WeightKg -= 2.5; }

    [RelayCommand]
    private async Task CompleteSetAsync(ExerciseSet set)
    {
        set.IsCompleted = !set.IsCompleted;
        set.IsSuggested = false;
        TotalSetsCompleted = Exercises.Sum(e => e.Sets.Count(s => s.IsCompleted));
        if (!set.IsCompleted) { SkipRestTimer(); return; }
        var parent = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        if (parent != null && !IsCreating) { await notifications.EnsureRestPermissionAsync(); StartExerciseRestTimer(parent); }
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
        if (i >= 0 && i < Exercises.Count - 1) { Exercises.Move(i, i + 1); Renumber(); }
    }

    private void Renumber() { for (int i = 0; i < Exercises.Count; i++) Exercises[i].Order = i + 1; }

    [RelayCommand]
    private async Task ReplaceExerciseAsync(WorkoutExercise ex)
    {
        replacingExercise = ex;
        await ShowSearchAsync();
    }

    [RelayCommand]
    private void StartExerciseRestTimer(WorkoutExercise ex)
    {
        restCts?.Cancel();
        activeRestDuration = Math.Clamp(ex.RestSeconds, 0, 300);
        var next = ex.Sets.Count > 0 && ex.Sets.All(s => s.IsCompleted)
            ? Exercises.Skip(Exercises.IndexOf(ex) + 1).FirstOrDefault(e => e.Sets.Any(s => !s.IsCompleted)) ?? ex : ex;
        notifications.ScheduleRest(activeRestDuration, next.ExerciseName, next.ImageUrl);
        IsRestPaused = false;
        if (activeRestDuration == 0) { SkipRestTimer(); return; }
        restCts = new CancellationTokenSource();
        var token = restCts.Token;
        restSecondsRemaining = activeRestDuration;
        IsRestTimerActive = true;
        RestTimerLabel = $"pausa {ex.ExerciseName}";
        RestTimerProgress = 1.0;
        RefreshRestTimerUI();
        workoutSession.UpdateRestTimer(true, RestTimerText);
        RunTimerAsync(token);
    }

    private void RunTimerAsync(CancellationToken token)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (token.IsCancellationRequested) return;
                        restSecondsRemaining = notifications.RestRemaining;
                        IsRestPaused = notifications.IsRestPaused;
                        RestTimerProgress = (double)restSecondsRemaining / Math.Max(1, activeRestDuration);
                        RefreshRestTimerUI();
                        IsRestTimerActive = restSecondsRemaining > 0;
                        workoutSession.UpdateRestTimer(IsRestTimerActive, RestTimerText);
                        if (!IsRestTimerActive) notifications.FinishRest();
                    });
                    if (notifications.RestRemaining <= 0) break;
                    await Task.Delay(250, token);
                }
            }
            catch (OperationCanceledException) { }
        }, token);
    }

    private void RefreshRestTimerUI() => RestTimerText = $"{restSecondsRemaining / 60:00}:{restSecondsRemaining % 60:00}";

    [RelayCommand]
    private void SkipRestTimer()
    {
        restCts?.Cancel();
        IsRestTimerActive = false;
        notifications.CancelRest();
        RestTimerProgress = 0;
        RestTimerText = "0s";
        workoutSession.UpdateRestTimer(false, "");
    }

    [RelayCommand]
    private async Task OpenExerciseDetailAsync(WorkoutExercise ex)
    {
        // Old plans/drafts may predate saved media and instructions.
        var catalog = await exerciseDbApi.GetCatalogAsync();
        var cached = catalog.FirstOrDefault(e => e.Id == ex.ExerciseId) ??
            catalog.FirstOrDefault(e => string.Equals(e.Name.Trim(), ex.ExerciseName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (cached != null)
        {
            if (string.IsNullOrWhiteSpace(ex.GifUrl)) ex.GifUrl = cached.ImageUrl;
            if (string.IsNullOrWhiteSpace(ex.ImageUrl)) ex.ImageUrl = cached.ImageUrl;
            if (ex.Instructions.Count == 0)
                ex.Instructions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(cached.InstructionsJson) ?? [];
        }
        SelectedExercise = ex;
        previousHistory = await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "");
        BuildExerciseProgress();
        IsExerciseDetailVisible = true;
    }
    private async Task EnrichExerciseAsync(WorkoutExercise ex)
    {
        var cached = await db.GetCachedExerciseByIdAsync(ex.ExerciseId);
        if (cached == null) return;
        if (string.IsNullOrWhiteSpace(ex.ImageUrl)) ex.ImageUrl = cached.ImageUrl;
        if (string.IsNullOrWhiteSpace(ex.GifUrl)) ex.GifUrl = cached.ImageUrl;
        if (ex.Instructions.Count == 0)
        {
            try { ex.Instructions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(cached.InstructionsJson) ?? []; }
            catch (System.Text.Json.JsonException) { }
        }
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
            Id = Mode == "plan" && !string.IsNullOrEmpty(PlanId) ? PlanId : Guid.NewGuid().ToString(),
            Name = name,
            Notes = WorkoutNotes,
            RestSeconds = RestDuration,
            Exercises = PlanSessionChanges.CopyExercises(Exercises),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await planService.SavePlanAsync(plan);

        ShowNotification($"Scheda \"{name}\" salvata!");
        workoutSession.End();
        notifications.CancelRest();
        await Shell.Current.GoToAsync("..");
    }

    private async Task<bool> SaveToPocketBaseAsync(string name)
    {
        var user = pb.CurrentUser;
        if (user == null)
        {
            ShowNotification("Accedi al tuo account prima di completare l'allenamento.");
            return false;
        }

        try
        {
            var exerciseData = Exercises.Select(e => new
            {
                id = e.Id,
                exerciseId = e.ExerciseId,
                name = e.ExerciseName,
                bodyPart = e.BodyPart,
                equipment = e.Equipment,
                notes = e.Notes,
                restSeconds = e.RestSeconds,
                sets = e.Sets.Select(s => new
                {
                    setNumber = s.SetNumber,
                    kind = s.Kind,
                    weightKg = s.WeightKg,
                    reps = s.Reps,
                    isCompleted = s.IsCompleted
                }).ToList()
            }).ToList();

            var volume = Exercises.Sum(e => e.Sets.Where(s => s.IsCompleted).Sum(s => s.WeightKg * s.Reps));
            var duration = (int)(DateTime.Now - workoutStartTime).TotalMinutes;

            var localWorkout = new Models.LocalWorkout
            {
                Id = Guid.NewGuid().ToString("N")[..15],
                UserId = user.Id,
                Name = name,
                Date = workoutStartTime.ToUniversalTime().ToString("o"),
                ExercisesJson = System.Text.Json.JsonSerializer.Serialize(Exercises.Select(e => e.ExerciseName).ToList()),
                Volume = volume,
                Duration = Math.Max(1, duration),
                Notes = WorkoutNotes ?? "",
                ExerciseDataJson = System.Text.Json.JsonSerializer.Serialize(exerciseData),
                UserName = user.Name,
                PhotosJson = System.Text.Json.JsonSerializer.Serialize(WorkoutPhotos.ToList()),
                PendingSync = true
            };
            // Durable local save is the success boundary, before any network request.
            await db.SaveWorkoutAsync(localWorkout);
            WeakReferenceMessenger.Default.Send(new WorkoutSavedMessage());

            try
            {
                if (connectivity.IsOnline && await pb.UploadWorkoutAsync(localWorkout))
                    await db.MarkSyncedAsync(localWorkout.Id);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[SaveWorkout Sync] {ex.GetType().Name}"); }
            WeakReferenceMessenger.Default.Send(new WorkoutSavedMessage());
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SaveWorkout] PB ex: {ex.Message}");
            ShowNotification("Salvataggio locale non riuscito. Riprova: l'allenamento è ancora aperto.");
            return false;
        }
    }

    [RelayCommand]
    private async Task MinimizeWorkoutAsync()
    {
        if (IsBusy) return;
        if (!await PersistPlanNotesAsync()) return;
        if (Exercises.Count > 0)
        {
            if (!await SaveDraftAsync()) return;
            workoutSession.Minimize();
        }
        else
        {
            workoutSession.End();
        }
        elapsedCts?.Cancel();
        restCts?.Cancel();
        await Shell.Current.GoToAsync("..");
    }

    private async Task<bool> SaveDraftAsync()
    {
        try
        {
            // Replace only this session's draft; an unrelated interrupted workout stays available.
            var oldDraftId = sessionDraftId;


            var exerciseData = Exercises.Select(e => new
            {
                id = e.Id,
                exerciseId = e.ExerciseId,
                name = e.ExerciseName,
                bodyPart = e.BodyPart,
                equipment = e.Equipment,
                notes = e.Notes,
                restSeconds = e.RestSeconds,
                sets = e.Sets.Select(s => new
                {
                    setNumber = s.SetNumber,
                    kind = s.Kind,
                    weightKg = s.WeightKg,
                    reps = s.Reps,
                    isCompleted = s.IsCompleted
                }).ToList()
            }).ToList();

            var volume = Exercises.Sum(e => e.Sets.Sum(s => s.WeightKg * s.Reps));
            var duration = (int)(DateTime.Now - workoutStartTime).TotalMinutes;

            var draft = new Models.LocalWorkout
            {
                Id = "draft_" + Guid.NewGuid().ToString()[..8],
                PlanId = IsPlanWorkout ? PlanId : "",
                UserId = pb.IsLoggedIn ? pb.CurrentUser?.Id ?? "" : "",
                Name = string.IsNullOrWhiteSpace(PlanNameInput) ? PlanName : PlanNameInput,
                Date = DateTime.UtcNow.ToString("o"),
                ExercisesJson = System.Text.Json.JsonSerializer.Serialize(Exercises.Select(e => e.ExerciseName).ToList()),
                ExerciseDataJson = System.Text.Json.JsonSerializer.Serialize(exerciseData),
                Volume = volume,
                Duration = Math.Max(1, duration),
                Notes = WorkoutNotes ?? "",
                UserName = pb.IsLoggedIn ? pb.CurrentUser?.Name ?? "" : "",
                PhotosJson = System.Text.Json.JsonSerializer.Serialize(WorkoutPhotos.ToList()),
                IsDraft = true
            };
            await db.SaveWorkoutAsync(draft);
            sessionDraftId = draft.Id;
            if (oldDraftId != null) await db.DeleteWorkoutAsync(oldDraftId);
            System.Diagnostics.Debug.WriteLine($"[Draft] saved id={draft.Id}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Draft] ex: {ex.Message}");
            ShowNotification("Impossibile salvare la bozza. L’allenamento resta aperto: riprova.");
            return false;
        }
    }

    private async Task<List<LocalWorkout>> GetDraftsAsync()
    {
        var all = await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "");
        return all.Where(w => w.IsDraft).ToList();
    }

    private async Task LoadDraftAsync()
    {
        planNotesReady = false;
        notesSaveCts?.Cancel();
        try
        {
            var drafts = await GetDraftsAsync();
            var draft = drafts.FirstOrDefault();
            if (draft == null) { Mode = "free"; return; }

            Exercises.Clear();
            var exerciseData = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(draft.ExerciseDataJson);

            if (exerciseData == null) throw new InvalidOperationException("Bozza non valida.");
            sessionDraftId = draft.Id;

            int order = 1;
            for (int i = 0; i < exerciseData.Count; i++)
            {
                var ed = exerciseData[i];
                var ex = new WorkoutExercise
                {
                    Id = ed.TryGetProperty("id", out var originalId) ? originalId.GetString() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(),
                    ExerciseId = ed.TryGetProperty("exerciseId", out var exerciseId) ? exerciseId.GetString() ?? "" : "",
                    ExerciseName = ed.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "",
                    BodyPart = ed.TryGetProperty("bodyPart", out var bp) ? bp.GetString() ?? "" : "",
                    Equipment = ed.TryGetProperty("equipment", out var eq) ? eq.GetString() ?? "" : "",
                    Notes = ed.TryGetProperty("notes", out var nt) ? nt.GetString() ?? "" : "",
                    RestSeconds = ed.TryGetProperty("restSeconds", out var rs) && rs.TryGetInt32(out var rsv) ? rsv : 90,
                    Order = order++
                };

                if (ed.TryGetProperty("sets", out var setsArr) && setsArr.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    int setNum = 1;
                    foreach (var s in setsArr.EnumerateArray())
                    {
                        ex.Sets.Add(new ExerciseSet
                        {
                            SetNumber = setNum++,
                            Kind = s.TryGetProperty("kind", out var kind) ? kind.GetString() ?? "normal" : "normal",
                            WeightKg = s.TryGetProperty("weightKg", out var wk) && wk.TryGetDouble(out var w) ? w : 0,
                            Reps = s.TryGetProperty("reps", out var rp) && rp.TryGetInt32(out var r) ? r : 0,
                            IsCompleted = s.TryGetProperty("isCompleted", out var ic) && ic.ValueKind == System.Text.Json.JsonValueKind.True
                        });
                    }
                }
                await EnrichExerciseAsync(ex);
                Exercises.Add(ex);
            }

            restoringDraft = true;
            PlanId = draft.PlanId;
            Mode = string.IsNullOrEmpty(draft.PlanId) ? "free" : "plan";
            restoringDraft = false;
            PlanName = draft.Name;
            PlanNameInput = draft.Name;
            WorkoutNotes = draft.Notes;
            workoutStartTime = DateTime.Now.AddMinutes(-draft.Duration);
            if (!string.IsNullOrEmpty(PlanId))
            {
                var original = (await planService.LoadPlansAsync()).FirstOrDefault(p => p.Id == PlanId);
                if (original != null) {
                    RestDuration = original.RestSeconds;
                    previousHistory = await db.GetWorkoutsAsync(pb.CurrentUser?.Id ?? "");
                    foreach (var ex in original.Exercises) PreviousPerformance.Apply(previousHistory, ex);
                    originalPlanSignature = PlanSessionChanges.Signature(original.Name, original.Notes, original.RestSeconds, original.Exercises);
                    planNotesOwner = original.UserId;
                    planNotesReady = true;
                    QueueNotesSave();
                }
            }
            if (!string.IsNullOrWhiteSpace(draft.PhotosJson) && draft.PhotosJson != "[]")
            {
                var photos = System.Text.Json.JsonSerializer.Deserialize<List<string>>(draft.PhotosJson);
                if (photos != null)
                    foreach (var p in photos)
                        WorkoutPhotos.Add(p);
                HasWorkoutPhotos = WorkoutPhotos.Count > 0;
            }

            System.Diagnostics.Debug.WriteLine($"[Draft] loaded {Exercises.Count} exercises from {draft.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Draft] Load ex: {ex.Message}");
            throw;
        }
    }

    [RelayCommand]
    private async Task FinishWorkoutAsync()
    {
        if (IsFinishingWorkout) return;
        IsFinishingWorkout = true;
        try { await FinishWorkoutCoreAsync(); }
        finally { IsFinishingWorkout = false; }
    }
    private async Task FinishWorkoutCoreAsync()
    {
        if (!await PersistPlanNotesAsync()) return;
        if (IsPlanWorkout && originalPlanSignature != null && CurrentPlanSignature() != originalPlanSignature && !planChoiceResolved)
        { IsPlanUpdateVisible = true; return; }
        if (IsPlanWorkout && updatePlanOnFinish)
        {
            try { await ApplySessionToPlanAsync(); updatePlanOnFinish = false; }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[UpdatePlan] {ex.GetType().Name}"); planChoiceResolved = false; ShowNotification("Impossibile aggiornare la scheda. La sessione resta aperta: riprova."); return; }
        }
        if (Exercises.Count > 0 && IsTimerRunning)
        {
            if (!await SaveToPocketBaseAsync(string.IsNullOrWhiteSpace(PlanNameInput) ? PlanName : PlanNameInput.Trim())) { planChoiceResolved = false; return; }
            // Remove recovery data only after the finished workout is durable.
            if (sessionDraftId != null) await db.DeleteWorkoutAsync(sessionDraftId);
        }

        elapsedCts?.Cancel();
        restCts?.Cancel();
        notifications.CancelRest();

        planNotesReady = false;
        notesSaveCts?.Cancel();
        workoutSession.End();
        await Shell.Current.GoToAsync("..");
    }

    private async Task ApplySessionToPlanAsync()
    {
        var plan = (await planService.LoadPlansAsync()).FirstOrDefault(p => p.Id == PlanId)
            ?? throw new InvalidOperationException("Scheda non disponibile.");
        plan.Name = (string.IsNullOrWhiteSpace(PlanNameInput) ? PlanName : PlanNameInput).Trim();
        plan.Notes = WorkoutNotes;
        plan.RestSeconds = RestDuration;
        plan.Exercises = PlanSessionChanges.CopyExercises(Exercises);
        plan.UpdatedAt = DateTime.Now;
        await planService.SavePlanAsync(plan);
        originalPlanSignature = CurrentPlanSignature();
    }

    private void ShowNotification(string message) => NotificationMessage = message;
    [RelayCommand] private void DismissNotification() => NotificationMessage = "";
    [RelayCommand] private async Task FinishOrSaveAsync()
    {
        if (IsBusy) return;
        if (IsCreating) await SaveWorkoutPlanAsync();
        else if (IsTimerRunning) await FinishWorkoutAsync();
        else ShowNotification("Avvia l’allenamento prima di terminarlo.");
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
                var e = DateTime.Now - workoutStartTime;
                ElapsedTime = $"{(int)e.TotalHours:D2}:{e.Minutes:D2}:{e.Seconds:D2}";
                workoutSession.UpdateElapsed(ElapsedTime);
                if (e.Seconds % 6 == 0) foreach (var exercise in Exercises) exercise.AdvanceExecutionTip();
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
    public string[] GetValues() => Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

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
