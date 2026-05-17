using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GymTracker.Mobile.Messages;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class SquadMember : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    [ObservableProperty] private bool isActive;
    [ObservableProperty] private bool hasWorkout;
    [ObservableProperty] private bool hasAvatar;
}

public partial class HomeViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private string streakCount = "0";
    [ObservableProperty] private string streakLabel = "Day streak";
    [ObservableProperty] private string streakDescription = "Start your first workout today!";
    [ObservableProperty] private string nextWorkout = "Create a plan";
    [ObservableProperty] private string workoutDuration = "Tap to start";
    [ObservableProperty] private string randomPlanId = string.Empty;
    [ObservableProperty] private bool hasProtocol;
    [ObservableProperty] private ObservableCollection<SquadMember> squad = new();

    public HomeViewModel(PocketBaseService pb)
    {
        this.pb = pb;
        HasData = true;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await LoadAsync());
        });
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await CalculateStreakAsync();
        await LoadSquadAsync();
        LoadRandomPlan();
    }

    private void LoadRandomPlan()
    {
        var plans = PlanStore.LoadPlans();
        if (plans.Count > 0)
        {
            var rng = new Random();
            var plan = plans[rng.Next(plans.Count)];
            NextWorkout = plan.Name;
            var estimatedMin = plan.Exercises.Sum(e => e.Sets.Count) * 3 + plan.RestSeconds / 60 * plan.Exercises.Count;
            WorkoutDuration = $"{plan.Exercises.Count} ex, ~{estimatedMin}m";
            RandomPlanId = plan.Id;
            HasProtocol = true;
        }
        else
        {
            NextWorkout = "Create a plan";
            WorkoutDuration = "Tap to start";
            HasProtocol = false;
        }
    }

    private async Task CalculateStreakAsync()
    {
        try
        {
            if (!pb.IsLoggedIn) return;
            var workouts = await pb.GetMyWorkoutsAsync(365);
            if (workouts.Count == 0) return;

            var dates = workouts
                .Select(w => { DateTime.TryParse(w.Date, out var d); return d; })
                .Where(d => d != default)
                .Select(d => d.ToLocalTime().Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            int streak = 0;
            var today = DateTime.Now.Date;
            var check = today;

            foreach (var d in dates)
            {
                if (d == check) { streak++; check = check.AddDays(-1); }
                else if (d < check) break;
            }

            StreakCount = streak.ToString();
            StreakLabel = streak == 1 ? "Day streak" : "Days streak";
            StreakDescription = streak > 0
                ? $"You trained {streak} day{(streak > 1 ? "s" : "")} in a row. Keep going!"
                : "Start your first workout today!";
        }
        catch { }
    }

    private async Task LoadSquadAsync()
    {
        try
        {
            if (!pb.IsLoggedIn) return;
            var followingIds = await pb.GetFollowingUserIdsAsync();
            Squad.Clear();
            if (followingIds.Count == 0) return;

            var members = new List<(string id, string name, string avatarUrl)>();
            foreach (var id in followingIds.Take(4))
            {
                try
                {
                    var users = await pb.SearchUsersAsync(id);
                    var u = users.FirstOrDefault(x => x.Id == id);
                    var name = u?.Name ?? id[..2].ToUpper();
                    var avatarUrl = "";
                    if (u != null && !string.IsNullOrWhiteSpace(u.Avatar))
                        avatarUrl = pb.GetFileUrl(u.CollectionId, u.Id, u.Avatar);
                    members.Add((id, name, avatarUrl));
                }
                catch { members.Add((id, id[..2].ToUpper(), "")); }
            }

            // Check which members have workouts
            try
            {
                var allWorkouts = await pb.GetMyWorkoutsAsync(100);
                var _ = allWorkouts; // keep reference to avoid issues
                var followedWorkouts = await pb.GetFollowedWorkoutsAsync();
                var activeUserIds = followedWorkouts.Select(w => w.User).Distinct().ToHashSet();

                for (int i = 0; i < members.Count; i++)
                {
                    Squad.Add(new SquadMember
                    {
                        Name = members[i].name,
                        UserId = members[i].id,
                        Initial = members[i].name.Length >= 1 ? members[i].name[..1].ToUpper() : "?",
                        AvatarUrl = members[i].avatarUrl,
                        HasAvatar = !string.IsNullOrWhiteSpace(members[i].avatarUrl),
                        IsActive = i < 2,
                        HasWorkout = activeUserIds.Contains(members[i].id)
                    });
                }
            }
            catch
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Squad.Add(new SquadMember
                    {
                        Name = members[i].name,
                        UserId = members[i].id,
                        Initial = members[i].name.Length >= 1 ? members[i].name[..1].ToUpper() : "?",
                        AvatarUrl = members[i].avatarUrl,
                        HasAvatar = !string.IsNullOrWhiteSpace(members[i].avatarUrl),
                        IsActive = i < 2,
                        HasWorkout = false
                    });
                }
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task StartWorkoutAsync() => await Shell.Current.GoToAsync("startSession");

    [RelayCommand]
    private async Task OpenNotificationsAsync() => await Shell.Current.GoToAsync("notifications");

    [RelayCommand]
    private async Task OpenSettingsAsync() => await Shell.Current.GoToAsync("settings");

    [RelayCommand]
    private async Task OpenFeedAsync() => await Shell.Current.GoToAsync("//feed");

    [RelayCommand]
    private async Task OpenNextWorkoutAsync()
    {
        if (!string.IsNullOrWhiteSpace(RandomPlanId))
        {
            await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
            {
                ["mode"] = "plan",
                ["planId"] = RandomPlanId
            });
        }
        else
        {
            await Shell.Current.GoToAsync("startSession");
        }
    }
}
