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
    [ObservableProperty] private ImageSource? avatarSource;
    [ObservableProperty] private bool isActive;
    [ObservableProperty] private bool hasWorkout;
    [ObservableProperty] private bool hasAvatar;
}

public partial class HomeViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private string streakCount = "0";
    [ObservableProperty] private string streakLabel = "Week streak";
    [ObservableProperty] private string streakDescription = "Start your first workout today!";
    [ObservableProperty] private string nextWorkout = "Create a plan";
    [ObservableProperty] private string workoutDuration = "Tap to start";
    [ObservableProperty] private string randomPlanId = string.Empty;
    [ObservableProperty] private bool hasProtocol;
    [ObservableProperty] private ObservableCollection<SquadMember> squad = new();
    [ObservableProperty] private string userInitials = "GT";
    [ObservableProperty] private string userAvatarUrl = string.Empty;
    [ObservableProperty] private ImageSource? userAvatarSource;
    [ObservableProperty] private bool hasUserAvatar;

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
        LoadUserInfo();
        await CalculateStreakAsync();
        await LoadSquadAsync();
        LoadRandomPlan();
    }

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
            if (workouts.Count == 0) { StreakCount = "0"; return; }

            var dates = workouts
                .Select(w => { DateTime.TryParse(w.Date, out var d); return d; })
                .Where(d => d != default)
                .Select(d => d.ToLocalTime().Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (dates.Count == 0) { StreakCount = "0"; return; }

            var mostRecent = dates.First();
            var daysSinceLastWorkout = (DateTime.Now.Date - mostRecent).Days;
            if (daysSinceLastWorkout > 7) { StreakCount = "0"; return; }

            var weekStart = mostRecent.AddDays(-(int)mostRecent.DayOfWeek + 1);
            if (mostRecent.DayOfWeek == DayOfWeek.Sunday)
                weekStart = mostRecent.AddDays(-6);

            int streak = 0;
            while (true)
            {
                var weekEnd = weekStart.AddDays(6);
                if (!dates.Any(d => d >= weekStart && d <= weekEnd)) break;
                streak++;
                weekStart = weekStart.AddDays(-7);
            }

            StreakCount = streak.ToString();
            StreakLabel = streak == 1 ? "Week streak" : "Weeks streak";
            StreakDescription = streak > 0
                ? $"You trained {streak} week{(streak > 1 ? "s" : "")} in a row. Keep going!"
                : "Start your first workout today!";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Home Streak] ex: {ex.Message}");
        }
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Home Squad] userRes err: {ex.Message}");
                    members.Add((id, id[..2].ToUpper(), ""));
                }
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
                        AvatarSource = !string.IsNullOrWhiteSpace(members[i].avatarUrl)
                            ? ImageSource.FromUri(new Uri(members[i].avatarUrl)) : null,
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
                        AvatarSource = !string.IsNullOrWhiteSpace(members[i].avatarUrl)
                            ? ImageSource.FromUri(new Uri(members[i].avatarUrl)) : null,
                        HasAvatar = !string.IsNullOrWhiteSpace(members[i].avatarUrl),
                        IsActive = i < 2,
                        HasWorkout = false
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Home Squad] ex: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task StartWorkoutAsync() => await Shell.Current.GoToAsync("startSession");

    [RelayCommand]
    private async Task OpenNotificationsAsync() => await Shell.Current.GoToAsync("notifications");

    [RelayCommand]
    private async Task OpenProfileAsync() => await Shell.Current.GoToAsync("profile");

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
