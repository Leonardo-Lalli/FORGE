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
    public bool IsMonthLabel { get; set; }
}

public partial class StatsViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;
    private List<Models.Dto.LoggedWorkoutRecord> allWorkouts = new();
    private DateTime lastCalendarDate = DateTime.MinValue;

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
        LoadUserInfo();
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

        var workoutDates = workouts
            .Select(w => { DateTime.TryParse(w.Date, out var d); return d; })
            .Where(d => d != default)
            .ToList();

        if (workoutDates.Count == 0) return;

        var allDays = workoutDates
            .GroupBy(d => d.Date)
            .ToDictionary(g => g.Key, g => g.Sum(w => workouts
                .Where(x => DateTime.TryParse(x.Date, out var xd) && xd.Date == g.Key)
                .Sum(x => x.Volume)));

        var minDate = workoutDates.Min().Date;
        var maxDate = DateTime.Now.Date;

        // Build week buckets (Mon-Sun)
        var weeks = new List<(DateTime WeekStart, double Volume, string Label)>();
        var cursor = minDate;
        while (cursor.DayOfWeek != DayOfWeek.Monday && cursor > minDate.AddDays(-7))
            cursor = cursor.AddDays(-1);
        cursor = minDate.AddDays(-(int)minDate.DayOfWeek + 1);
        if (cursor > minDate) cursor = cursor.AddDays(-7);

        var end = maxDate.AddDays(7);
        while (cursor <= end)
        {
            var weekEnd = cursor.AddDays(6);
            double vol = 0;
            for (var d = cursor; d <= weekEnd && d <= maxDate; d = d.AddDays(1))
            {
                if (allDays.TryGetValue(d.Date, out var v))
                    vol += v;
            }
            weeks.Add((cursor, vol, $"{cursor:dd/MM}"));
            cursor = cursor.AddDays(7);
        }

        // Keep only weeks from the last 4 months for clarity
        var cutoff = maxDate.AddMonths(-4);
        weeks = weeks.Where(w => w.WeekStart >= cutoff).ToList();

        double maxVol = weeks.Any() ? weeks.Max(w => w.Volume) : 1;
        string currentMonth = "";

        foreach (var w in weeks)
        {
            var monthName = w.WeekStart.ToString("MMM").ToUpper();
            if (monthName != currentMonth)
            {
                currentMonth = monthName;
                BarLabels.Add(new LabelEntry { Text = monthName, IsMonthLabel = true });
                BarHeights.Add(new BarEntry { Height = 0 });
            }
            BarLabels.Add(new LabelEntry { Text = w.Label });
            BarHeights.Add(new BarEntry { Height = maxVol > 0 ? (w.Volume / maxVol) * 140 : 0 });
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

    private void LoadUserInfo()
    {
        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            var u = pb.CurrentUser;
            UserInitials = (u.Name?.Length >= 2) ? u.Name[..2].ToUpper() : (u.Email?.Length >= 2 ? u.Email[..2].ToUpper() : "GT");
            if (!string.IsNullOrWhiteSpace(u.Avatar))
            {
                UserAvatarUrl = pb.GetFileUrl(u.CollectionId, u.Id, u.Avatar);
                UserAvatarSource = ImageSource.FromUri(new Uri(UserAvatarUrl));
                HasUserAvatar = true;
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
        var today = DateTime.Now;
        if (lastCalendarDate != DateTime.MinValue && lastCalendarDate.Date != today.Date)
        {
            foreach (var day in CalendarDays)
                day.IsToday = day.Day == today.Day.ToString();
            lastCalendarDate = today;
            CalendarMonth = today.ToString("MMMM yyyy");
            return;
        }
        lastCalendarDate = today;

        CalendarDays.Clear();
        CalendarMonth = today.ToString("MMMM yyyy");
        var firstOfMonth = new DateTime(today.Year, today.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var startDay = (int)firstOfMonth.DayOfWeek;
        if (startDay == 0) startDay = 7;

        var workoutDates = allWorkouts
            .Select(w => { DateTime.TryParse(w.Date, out var d); return d.Date; })
            .Where(d => d != default)
            .ToHashSet();

        // Empty cells before first day
        for (int i = 1; i < startDay; i++)
            CalendarDays.Add(new CalendarDay { Day = "", HasWorkout = false });

        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(today.Year, today.Month, d);
            CalendarDays.Add(new CalendarDay
            {
                Day = d.ToString(),
                HasWorkout = workoutDates.Contains(date),
                IsToday = date == today.Date
            });
        }
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
}
