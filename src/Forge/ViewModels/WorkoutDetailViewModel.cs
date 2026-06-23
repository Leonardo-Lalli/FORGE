using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Services;

namespace Forge.ViewModels;

public partial class DetailExercise : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);
    public ObservableCollection<DetailSet> Sets { get; set; } = new();
}

public partial class DetailSet : ObservableObject
{
    public int SetNumber { get; set; }
    public double WeightKg { get; set; }
    public int Reps { get; set; }
    public bool IsCompleted { get; set; }
    public string CompletedIcon => IsCompleted ? "\u2713" : "";
}

[QueryProperty(nameof(WorkoutId), "workoutId")]
[QueryProperty(nameof(Source), "source")]
public partial class WorkoutDetailViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;
    private readonly DatabaseService db;

    [ObservableProperty] private string workoutId = string.Empty;
    [ObservableProperty] private string source = "pocketbase";

    [ObservableProperty] private string workoutName = string.Empty;
    [ObservableProperty] private string workoutDate = string.Empty;
    [ObservableProperty] private string duration = "0";
    [ObservableProperty] private string volume = "0";
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private int likes;
    [ObservableProperty] private string userName = string.Empty;

    [ObservableProperty] private ObservableCollection<DetailExercise> exercises = new();
    [ObservableProperty] private ObservableCollection<string> photos = new();
    [ObservableProperty] private bool hasPhotos;
    [ObservableProperty] private bool hasNotes;
    [ObservableProperty] private bool hasLikes;
    [ObservableProperty] private string selectedPhoto = string.Empty;
    [ObservableProperty] private bool isPhotoExpanded;

    public WorkoutDetailViewModel(PocketBaseService pb, DatabaseService db)
    {
        this.pb = pb;
        this.db = db;
        HasData = true;
    }

    partial void OnWorkoutIdChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _ = LoadAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    System.Diagnostics.Debug.WriteLine($"[WorkoutDetail Load] ex: {t.Exception?.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(WorkoutId)) return;
        SetLoading();

        try
        {
            if (Source == "local")
                await LoadFromLocalAsync();
            else
                await LoadFromPocketBaseAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WorkoutDetail] Load ex: {ex.Message}");
            SetError("Errore nel caricamento dell'allenamento.");
        }
    }

    private async Task LoadFromLocalAsync()
    {
        var workouts = await db.GetWorkoutsAsync();
        var w = workouts.FirstOrDefault(x => x.Id == WorkoutId);
        if (w == null)
        {
            SetError("Allenamento non trovato.");
            return;
        }

        WorkoutName = w.Name;
        WorkoutDate = DateTime.TryParse(w.Date, out var d) ? d.ToString("dd MMM yyyy HH:mm") : w.Date;
        Duration = $"{w.Duration}min";
        Volume = $"{w.Volume:F0} kg";
        Notes = w.Notes;
        HasNotes = !string.IsNullOrWhiteSpace(w.Notes);
        UserName = w.UserName;

        ParseExerciseData(w.ExerciseDataJson);
        ParsePhotos(w.PhotosJson);

        SetSuccess(true);
    }

    private async Task LoadFromPocketBaseAsync()
    {
        var record = await pb.GetWorkoutByIdAsync(WorkoutId);
        if (record == null)
        {
            SetError("Allenamento non trovato su PocketBase.");
            return;
        }

        WorkoutName = record.Name;
        WorkoutDate = DateTime.TryParse(record.Date, out var d) ? d.ToString("dd MMM yyyy HH:mm") : record.Date;
        Duration = $"{record.Duration}min";
        Volume = $"{record.Volume:F0} kg";
        Notes = record.Notes;
        HasNotes = !string.IsNullOrWhiteSpace(record.Notes);
        Likes = record.Likes;
        HasLikes = record.Likes > 0;
        UserName = record.UserName;

        ParseExerciseData(record.ExerciseData);
        Photos = new ObservableCollection<string>(record.Photos);
        HasPhotos = Photos.Count > 0;

        SetSuccess(true);
    }

    private void ParseExerciseData(string exerciseDataJson)
    {
        Exercises.Clear();
        if (string.IsNullOrWhiteSpace(exerciseDataJson)) return;

        try
        {
            using var doc = JsonDocument.Parse(exerciseDataJson);
            var root = doc.RootElement;
            var items = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : default;

            foreach (var item in items)
            {
                var ex = new DetailExercise();
                if (item.TryGetProperty("name", out var n)) ex.Name = n.GetString() ?? "";
                if (item.TryGetProperty("bodyPart", out var bp)) ex.BodyPart = bp.GetString() ?? "";
                if (item.TryGetProperty("equipment", out var eq)) ex.Equipment = eq.GetString() ?? "";
                if (item.TryGetProperty("notes", out var nt)) ex.Notes = nt.GetString() ?? "";

                if (item.TryGetProperty("sets", out var setsArr) && setsArr.ValueKind == JsonValueKind.Array)
                {
                    int i = 1;
                    foreach (var s in setsArr.EnumerateArray())
                    {
                        var set = new DetailSet { SetNumber = i++ };
                        if (s.TryGetProperty("weightKg", out var wk) && wk.TryGetDouble(out var w)) set.WeightKg = w;
                        if (s.TryGetProperty("reps", out var rp) && rp.TryGetInt32(out var r)) set.Reps = r;
                        if (s.TryGetProperty("isCompleted", out var ic) && ic.ValueKind == JsonValueKind.True) set.IsCompleted = true;
                        ex.Sets.Add(set);
                    }
                }
                Exercises.Add(ex);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WorkoutDetail] ParseExerciseData ex: {ex.Message}");
        }
    }

    private void ParsePhotos(string photosJson)
    {
        Photos.Clear();
        if (string.IsNullOrWhiteSpace(photosJson) || photosJson == "[]") return;

        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(photosJson);
            if (list != null)
                foreach (var p in list)
                    Photos.Add(p);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[WorkoutDetail] ParsePhotos ex: {ex.Message}"); }
        HasPhotos = Photos.Count > 0;
    }

    [RelayCommand]
    private void ExpandPhoto(string photo)
    {
        SelectedPhoto = photo;
        IsPhotoExpanded = true;
    }

    [RelayCommand]
    private void ClosePhoto()
    {
        IsPhotoExpanded = false;
        SelectedPhoto = string.Empty;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
