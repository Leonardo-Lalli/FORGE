using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Services;
using Forge.Models;

namespace Forge.ViewModels;

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
    public bool IsMonthLabel { get; set; }
}

public partial class StatsViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;
    private readonly DatabaseService db;
    public ObservableCollection<VolumeBar> VolumeBars { get; } = new();
    private List<Models.Dto.LoggedWorkoutRecord> allWorkouts = new();
    [ObservableProperty] private ObservableCollection<CalendarMonthBlock> calendarMonths = new();
    [ObservableProperty] private int calendarHeight = 180;
    [ObservableProperty] private ObservableCollection<CalendarWorkout> dayWorkouts = new();
    [ObservableProperty] private bool isDayWorkoutsVisible;

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
    [ObservableProperty] private ObservableCollection<LikeNotification> likeNotifications = new();
    [ObservableProperty] private bool hasLikes;
    [ObservableProperty] private ObservableCollection<CalendarDay> calendarDays = new();
    [ObservableProperty] private string calendarMonth = string.Empty;
    [ObservableProperty] private string userInitials = "GT";
    [ObservableProperty] private string userAvatarUrl = string.Empty;
    [ObservableProperty] private ImageSource? userAvatarSource;
    [ObservableProperty] private bool hasUserAvatar;

    public StatsViewModel(PocketBaseService pb, DatabaseService db)
    {
        this.pb = pb;
        this.db = db;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await LoadStatsAsync(true));
        });
    }

    [RelayCommand]
    private Task LoadAsync() => LoadStatsAsync(false);
    private async Task LoadStatsAsync(bool localOnly)
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
            allWorkouts = await WorkoutHistory.LoadOwnAsync(pb, db, true, allWorkouts);
            ApplyFilter(SelectedFilter);
            BuildCalendar();
            if (!localOnly) { var remote = await WorkoutHistory.LoadOwnAsync(pb, db); allWorkouts = await WorkoutHistory.LoadOwnAsync(pb, db, true, remote); await LoadUserInfoAsync(); }
            System.Diagnostics.Debug.WriteLine($"[Stats] GetMyWorkoutsAsync returned {allWorkouts.Count} items");
            if (allWorkouts.Count == 0)
            {
                StatsError = "Le statistiche compariranno dopo il tuo primo allenamento.";
                HasStatsError = true;
                IsEmptyState = true;
                HasData = false;
            }
            else
            {
                HasData = true;
            }
            ApplyFilter(SelectedFilter);
            BuildLikeNotifications();
            BuildCalendar();
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
        BuildCalendar();
    }

    private void ApplyFilter(string filter)
    {
        if (allWorkouts.Count == 0) { VolumeBars.Clear(); TopLifts.Clear(); TotalHours = "0h"; TotalVolume = "0 kg"; Workouts = "0"; return; }

        var cutoff = StatsPeriod.Start(filter, DateTime.Now.Date, allWorkouts.Select(w => AchievementEvaluator.ParseDate(w.Date)).OfType<DateTime>().DefaultIfEmpty(DateTime.Now.Date).Min());

        var filtered = allWorkouts
            .Where(w =>
            {
                var d = AchievementEvaluator.ParseDate(w.Date);
                return d.HasValue && d.Value >= cutoff;
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
        VolumeBars.Clear();
        foreach (var bar in VolumeChart.Build(workouts, filter, DateTime.Now.Date))
            VolumeBars.Add(new VolumeBar { Height = bar.Height, Label = bar.Label, VolumeLabel = $"{bar.Volume:0.#}" });
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Stats BuildTopLifts] ex: {ex.Message}");
            }
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

    private async Task LoadUserInfoAsync()
    {
        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            var u = pb.CurrentUser;
            UserInitials = (u.Name?.Length >= 2) ? u.Name[..2].ToUpper() : (u.Email?.Length >= 2 ? u.Email[..2].ToUpper() : "GT");
            if (!string.IsNullOrWhiteSpace(u.Avatar))
            {
                UserAvatarSource = await pb.DownloadAvatarAsync(u.CollectionId, u.Id, u.Avatar);
                HasUserAvatar = UserAvatarSource != null;
            }
            else { UserAvatarSource = null; HasUserAvatar = false; }
        }
    }

    private void BuildLikeNotifications()
    {
        LikeNotifications.Clear();
        foreach (var w in allWorkouts.Where(w => w.Likes > 0))
        {
            LikeNotifications.Add(new LikeNotification
            {
                WorkoutName = w.Name,
                LikeCount = w.Likes,
                Date = w.Date
            });
        }
        HasLikes = LikeNotifications.Count > 0;
    }

    private void BuildCalendar()
    {
        var today = DateTime.Now.Date;
        var dated = allWorkouts.Select(w => (Workout: w, Date: AchievementEvaluator.ParseDate(w.Date)))
            .Where(w => w.Date.HasValue).GroupBy(w => w.Date!.Value.Date).ToDictionary(g => g.Key, g => g.Select(w => w.Workout).ToList());
        var start = StatsPeriod.Start(SelectedFilter, today, dated.Keys.DefaultIfEmpty(today).Min());
        var blocks = new List<CalendarMonthBlock>();
        var cursor = SelectedFilter == "week" ? start : new DateTime(start.Year, start.Month, 1);
        do
        {
            var end = SelectedFilter == "week" ? cursor.AddDays(6) : cursor.AddMonths(1).AddDays(-1);
            var days = new List<CalendarDay>();
            for (var i = 0; i < ((int)cursor.DayOfWeek + 6) % 7; i++) days.Add(new());
            for (var date = cursor; date <= end; date = date.AddDays(1))
            {
                var workouts = dated.GetValueOrDefault(date) ?? [];
                days.Add(new CalendarDay { Day = date.Day.ToString(), IsToday = date == today, HasWorkout = workouts.Count > 0,
                    WorkoutName = workouts.FirstOrDefault()?.Name ?? "", Workouts = workouts.Select(w => new CalendarWorkout { Id = w.Id, Name = w.Name, Source = w.IsLocal ? "local" : "pocketbase" }).ToList() });
            }
            while (days.Count % 7 != 0) days.Add(new());
            blocks.Add(new CalendarMonthBlock { Title = SelectedFilter == "week" ? $"{cursor:dd MMM} – {end:dd MMM yyyy}" : cursor.ToString("MMMM yyyy"), Days = new(days), Height = days.Count / 7 * 72 });
            cursor = cursor.AddMonths(1);
        } while (SelectedFilter != "week" && cursor <= today);
        CalendarHeight = SelectedFilter == "week" ? 170 : 520;
        CalendarMonths = new(blocks);
    }
    [RelayCommand] private async Task OpenCalendarDayAsync(CalendarDay day)
    {
        if (day.Workouts.Count == 1) await OpenCalendarWorkoutAsync(day.Workouts[0]);
        else if (day.Workouts.Count > 1) { DayWorkouts = new(day.Workouts); IsDayWorkoutsVisible = true; }
    }
    [RelayCommand] private void CloseDayWorkouts() => IsDayWorkoutsVisible = false;
    [RelayCommand] private async Task OpenCalendarWorkoutAsync(CalendarWorkout workout)
    {
        IsDayWorkoutsVisible = false;
        await Shell.Current.GoToAsync($"workoutDetail?workoutId={Uri.EscapeDataString(workout.Id)}&source={workout.Source}");
    }
}

public partial class LikeNotification : ObservableObject
{
    public string WorkoutName { get; set; } = "";
    public int LikeCount { get; set; }
    public string Date { get; set; } = "";
    public string DisplayText => $"{WorkoutName} — {LikeCount} like{(LikeCount > 1 ? "s" : "")}";
}

public partial class CalendarDay : ObservableObject
{
    public string Day { get; set; } = "";
    [ObservableProperty] private bool hasWorkout;
    [ObservableProperty] private bool isToday;
    public string WorkoutName { get; set; } = "";
    public List<CalendarWorkout> Workouts { get; set; } = [];
}
public class CalendarMonthBlock { public string Title { get; set; } = ""; public ObservableCollection<CalendarDay> Days { get; set; } = new(); public int Height { get; set; } }
public class CalendarWorkout { public string Id { get; set; } = ""; public string Name { get; set; } = ""; public string Source { get; set; } = "pocketbase"; }

public class VolumeBar { public double Height { get; set; } public string Label { get; set; } = ""; public string VolumeLabel { get; set; } = ""; }
