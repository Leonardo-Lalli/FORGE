using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Models;

namespace Forge.Services;

public class AchievementService
{
    private readonly DatabaseService db;
    private List<AchievementState> states = new();

    public AchievementService(DatabaseService db)
    {
        this.db = db;
        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, (_, _) =>
        {
            _ = MainThread.InvokeOnMainThreadAsync(async () => await OnWorkoutSavedAsync());
        });
    }

    public async Task<List<(AchievementDefinition Def, AchievementState State)>> GetAllAsync()
    {
        await LoadStatesAsync();
        return AchievementsCatalog.All.Select(def =>
        {
            var state = states.FirstOrDefault(s => s.Id == def.Id)
                        ?? new AchievementState { Id = def.Id, MaxProgress = def.MaxProgress };
            return (def, state);
        }).ToList();
    }

    public async Task<int> GetUnlockedCountAsync()
    {
        await LoadStatesAsync();
        return states.Count(s => s.IsUnlocked);
    }

    public async Task<double> GetProgressPercentAsync()
    {
        await LoadStatesAsync();
        var total = AchievementsCatalog.All.Count;
        if (total == 0) return 0;
        return (double)states.Count(s => s.IsUnlocked) / total * 100;
    }

    public async Task<List<(AchievementDefinition Def, AchievementState State)>> GetRecentUnlocksAsync(int count = 3)
    {
        await LoadStatesAsync();
        return AchievementsCatalog.All
            .Select(def => (Def: def, State: states.FirstOrDefault(s => s.Id == def.Id)
                ?? new AchievementState { Id = def.Id }))
            .Where(x => x.State.IsUnlocked && !string.IsNullOrWhiteSpace(x.State.UnlockedAt))
            .OrderByDescending(x => x.State.UnlockedAt)
            .Take(count)
            .ToList();
    }

    private async Task OnWorkoutSavedAsync()
    {
        await LoadStatesAsync();

        var workouts = (await db.GetWorkoutsAsync()).Where(w => !w.IsDraft).ToList();
        var totalWorkouts = workouts.Count;
        var now = DateTime.UtcNow;

        // Try to unlock achievements
        IncrementAchievement("first_workout"); // Completa 1 allenamento
        SetProgress("member", totalWorkouts); // Completa X allenamenti
        SetProgress("veteran", totalWorkouts);
        SetProgress("unstoppable", totalWorkouts); // 30 giorni (approssimato)

        // Check "Comeback" - if last workout before this was >14 days ago
        var ordered = workouts.OrderByDescending(w => DateTime.TryParse(w.Date, out var d) ? d : DateTime.MinValue).ToList();
        if (ordered.Count >= 2)
        {
            var last = DateTime.TryParse(ordered[0].Date, out var d1) ? d1 : DateTime.MinValue;
            var prev = DateTime.TryParse(ordered[1].Date, out var d2) ? d2 : DateTime.MinValue;
            if ((last - prev).TotalDays >= 14)
                UnlockAchievement("comeback");
        }

        // Volume achievements
        var weeklyVolume = workouts
            .Where(w => DateTime.TryParse(w.Date, out var dd) && dd >= now.AddDays(-7))
            .Sum(w => w.Volume);
        SetProgress("iron_marathon", (int)weeklyVolume);

        var monthlyVolume = workouts
            .Where(w => DateTime.TryParse(w.Date, out var dd) && dd >= now.AddDays(-30))
            .Sum(w => w.Volume);
        SetProgress("ton_week", (int)monthlyVolume);

        // Single workout volume
        var latest = ordered.FirstOrDefault();
        if (latest != null)
            SetProgress("max_weight", (int)latest.Volume);

        // Check exercises variety in latest workout
        if (!string.IsNullOrWhiteSpace(latest?.ExercisesJson))
        {
            try
            {
                var exList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(latest.ExercisesJson);
                if (exList != null)
                {
                    SetProgress("explorer", exList.Count);
                    SetProgress("encyclopedia", exList.Distinct().Count());
                }
            }
            catch { }
        }

        // Time-based achievements
        var localNow = now.ToLocalTime();
        if (localNow.Hour < 7) UnlockAchievement("early_bird");
        if (localNow.Hour >= 22) UnlockAchievement("night_owl");
        if (localNow.Hour >= 12 && localNow.Hour < 14) UnlockAchievement("lunch_break");

        // Weekend warrior
        if (localNow.DayOfWeek == DayOfWeek.Saturday || localNow.DayOfWeek == DayOfWeek.Sunday)
        {
            var weekendWorkouts = workouts
                .Where(w => DateTime.TryParse(w.Date, out var dd) && dd.ToLocalTime().DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                .Count();
            SetProgress("weekend_warrior", weekendWorkouts);
        }

        // Duration achievements
        var totalDuration = workouts.Sum(w => w.Duration);
        SetProgress("milestone", totalDuration); // 100 hours = 6000 min (diviso per 60 dopo)

        // Photo achievements
        var withPhotos = workouts.Count(w => !string.IsNullOrWhiteSpace(w.PhotosJson) && w.PhotosJson != "[]");
        SetProgress("narcissus", 1);
        SetProgress("photographer", withPhotos);

        // Birthday check
        var registrationDate = workouts.Min(w => DateTime.TryParse(w.Date, out var d) ? d : DateTime.MaxValue);
        if (registrationDate != DateTime.MaxValue && (now - registrationDate).TotalDays >= 365)
            UnlockAchievement("anniversary");

        await SaveStatesAsync();

        // Check "Divinità del Fitness" - all others unlocked
        var allDefs = AchievementsCatalog.All.Where(a => a.Id != "legend").ToList();
        var allUnlocked = allDefs.All(def => states.Any(s => s.Id == def.Id && s.IsUnlocked));
        if (allUnlocked)
            UnlockAchievement("legend");

        await SaveStatesAsync();
    }

    public async Task CheckLikeAchievementsAsync()
    {
        await LoadStatesAsync();
        // Like checks would go here when likes are tracked locally
        await SaveStatesAsync();
    }

    public async Task CheckFollowAchievementsAsync(int followingCount)
    {
        await LoadStatesAsync();
        SetProgress("networker", followingCount);
        await SaveStatesAsync();
    }

    private void IncrementAchievement(string id)
    {
        var state = states.FirstOrDefault(s => s.Id == id);
        if (state == null)
        {
            state = new AchievementState { Id = id, MaxProgress = AchievementsCatalog.All.FirstOrDefault(a => a.Id == id)?.MaxProgress ?? 1 };
            states.Add(state);
        }
        state.Progress++;
        if (state.Progress >= state.MaxProgress && !state.IsUnlocked)
        {
            state.IsUnlocked = true;
            state.UnlockedAt = DateTime.UtcNow.ToString("o");
        }
    }

    private void SetProgress(string id, int progress)
    {
        var state = states.FirstOrDefault(s => s.Id == id);
        if (state == null)
        {
            var def = AchievementsCatalog.All.FirstOrDefault(a => a.Id == id);
            state = new AchievementState { Id = id, MaxProgress = def?.MaxProgress ?? 1 };
            states.Add(state);
        }
        state.Progress = Math.Max(state.Progress, progress);
        if (state.Progress >= state.MaxProgress && !state.IsUnlocked)
        {
            state.IsUnlocked = true;
            state.UnlockedAt = DateTime.UtcNow.ToString("o");
        }
    }

    private void UnlockAchievement(string id)
    {
        var state = states.FirstOrDefault(s => s.Id == id);
        if (state == null)
        {
            var def = AchievementsCatalog.All.FirstOrDefault(a => a.Id == id);
            state = new AchievementState { Id = id, MaxProgress = def?.MaxProgress ?? 1 };
            states.Add(state);
        }
        if (!state.IsUnlocked)
        {
            state.IsUnlocked = true;
            state.Progress = state.MaxProgress;
            state.UnlockedAt = DateTime.UtcNow.ToString("o");
        }
    }

    private async Task LoadStatesAsync()
    {
        if (states.Count > 0) return;
        try
        {
            states = await db.GetAchievementsAsync();
        }
        catch { states = new(); }
    }

    private async Task SaveStatesAsync()
    {
        foreach (var s in states)
            await db.SaveAchievementAsync(s);
    }
}
