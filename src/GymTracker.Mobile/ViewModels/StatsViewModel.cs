using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GymTracker.Mobile.Messages;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class LiftEntry : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
}

public class BarEntry
{
    public double Height { get; set; }
}

public class LabelEntry
{
    public string Text { get; set; } = string.Empty;
}

public partial class StatsViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;
    private List<Models.Dto.LoggedWorkoutRecord> allWorkouts = new();

    [ObservableProperty] private string totalHours = "0";
    [ObservableProperty] private string totalHoursTrend = "";
    [ObservableProperty] private string workouts = "0";
    [ObservableProperty] private string workoutsTrend = "";
    [ObservableProperty] private string totalVolume = "0";
    [ObservableProperty] private string totalVolumeTrend = "";
    [ObservableProperty] private ObservableCollection<LiftEntry> topLifts = new();
    [ObservableProperty] private ObservableCollection<BarEntry> barHeights = new();
    [ObservableProperty] private ObservableCollection<LabelEntry> barLabels = new();
    [ObservableProperty] private string selectedFilter = "week";
    [ObservableProperty] private string statsError = string.Empty;
    [ObservableProperty] private bool hasStatsError;

    public StatsViewModel(PocketBaseService pb)
    {
        this.pb = pb;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await LoadAsync());
        });
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (!pb.IsLoggedIn)
        {
            StatsError = "Effettua il login per vedere le statistiche.";
            HasStatsError = true;
            IsEmptyState = true;
            HasData = false;
            return;
        }
        IsBusy = true;
        StatsError = string.Empty;
        HasStatsError = false;
        IsEmptyState = false;
        try
        {
            System.Diagnostics.Debug.WriteLine("[Stats] LoadAsync fetching workouts...");
            allWorkouts = await pb.GetMyWorkoutsAsync(365);
            System.Diagnostics.Debug.WriteLine($"[Stats] GetMyWorkoutsAsync returned {allWorkouts.Count} items");
            if (allWorkouts.Count == 0)
            {
                StatsError = "Nessun dato. Verifica su PocketBase:\n1) Collection 'logged_workouts' esiste\n2) API Rule List/Search = @request.auth.id != \"\"";
                HasStatsError = true;
                IsEmptyState = true;
                HasData = false;
            }
            else
            {
                HasData = true;
            }
            ApplyFilter(SelectedFilter);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Stats] LoadAsync ex: {ex}");
            StatsError = $"Errore caricamento statistiche: {ex.Message}";
            HasStatsError = true;
            IsEmptyState = true;
            HasData = false;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        SelectedFilter = filter;
        ApplyFilter(filter);
    }

    private void ApplyFilter(string filter)
    {
        if (allWorkouts.Count == 0) return;

        var cutoff = filter switch
        {
            "week" => DateTime.Now.Date.AddDays(-7),
            "month" => DateTime.Now.Date.AddMonths(-1),
            "3months" => DateTime.Now.Date.AddMonths(-3),
            "year" => DateTime.Now.Date.AddYears(-1),
            _ => DateTime.MinValue
        };

        var filtered = allWorkouts
            .Where(w =>
            {
                DateTime.TryParse(w.Date, out var d);
                return d >= cutoff || cutoff == DateTime.MinValue;
            })
            .ToList();

        Workouts = filtered.Count.ToString();
        TotalHours = $"{(int)filtered.Sum(w => w.Duration) / 60.0:0.#}h";
        TotalVolume = $"{filtered.Sum(w => w.Volume) / 1000.0:0.#}k kg";

        var prev = filtered.Count > 0 ? "+" : "";
        TotalHoursTrend = prev;
        WorkoutsTrend = prev;
        TotalVolumeTrend = prev;

        BuildBarChart(filtered, filter);
        BuildTopLifts(filtered);
    }

    private void BuildBarChart(List<Models.Dto.LoggedWorkoutRecord> workouts, string filter)
    {
        BarHeights.Clear();
        BarLabels.Clear();

        int buckets = filter switch { "week" => 7, "month" => 7, "3months" => 12, _ => 7 };
        var days = Enumerable.Range(0, buckets).Select(i => DateTime.Now.Date.AddDays(-i)).Reverse().ToList();

        var grouped = days.Select(d =>
        {
            var vol = workouts
                .Where(w => DateTime.TryParse(w.Date, out var wd) && wd.Date == d)
                .Sum(w => w.Volume);
            return (Date: d, Volume: vol);
        }).ToList();

        double maxVol = grouped.Any() ? grouped.Max(g => g.Volume) : 1;
        foreach (var g in grouped)
        {
            BarLabels.Add(new LabelEntry { Text = g.Date.ToString("ddd") });
            BarHeights.Add(new BarEntry { Height = maxVol > 0 ? (g.Volume / maxVol) * 140 : 0 });
        }
    }

    private void BuildTopLifts(List<Models.Dto.LoggedWorkoutRecord> workouts)
    {
        TopLifts.Clear();
        var jsonOpts = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var exerciseStats = new Dictionary<string, double>();
        foreach (var w in workouts)
        {
            if (string.IsNullOrWhiteSpace(w.ExerciseData)) continue;
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<List<JsonExercise>>(w.ExerciseData, jsonOpts);
                if (data == null) continue;
                foreach (var ex in data)
                {
                    var maxKg = ex.Sets?.Max(s => s.WeightKg) ?? 0;
                    if (maxKg <= 0) continue;
                    if (exerciseStats.ContainsKey(ex.Name))
                        exerciseStats[ex.Name] = Math.Max(exerciseStats[ex.Name], maxKg);
                    else
                        exerciseStats[ex.Name] = maxKg;
                }
            }
            catch { }
        }

        var top = exerciseStats.Where(kv => kv.Value > 0).OrderByDescending(kv => kv.Value).Take(5).ToList();

        foreach (var kv in top)
        {
            TopLifts.Add(new LiftEntry
            {
                Name = kv.Key,
                Label = "MAX",
                Weight = $"{kv.Value:0.#} kg"
            });
        }
    }

    private class JsonExercise
    {
        public string Name { get; set; } = "";
        public List<JsonSet>? Sets { get; set; }
    }

    private class JsonSet
    {
        public double WeightKg { get; set; }
        public int Reps { get; set; }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync() => await Shell.Current.GoToAsync("settings");

    [RelayCommand]
    private async Task OpenFriendRequestsAsync() => await Shell.Current.GoToAsync("friendRequests");

    [RelayCommand]
    private async Task OpenProfileAsync() => await Shell.Current.GoToAsync("profile");
}
