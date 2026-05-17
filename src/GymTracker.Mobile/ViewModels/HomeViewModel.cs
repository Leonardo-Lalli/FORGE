using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class SquadMember : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public partial class HomeViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private string streakCount = "0";
    [ObservableProperty] private string streakLabel = "Day streak";
    [ObservableProperty] private string streakDescription = "Start your first workout today!";
    [ObservableProperty] private ObservableCollection<SquadMember> squad = new();

    public HomeViewModel(PocketBaseService pb)
    {
        this.pb = pb;
        HasData = true;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await CalculateStreakAsync();
        await LoadSquadAsync();
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
            var names = new List<string>();
            foreach (var id in followingIds.Take(4))
            {
                try
                {
                    var users = await pb.SearchUsersAsync(id);
                    var u = users.FirstOrDefault(x => x.Id == id);
                    names.Add(u?.Name ?? id[..2].ToUpper());
                }
                catch { names.Add(id[..2].ToUpper()); }
            }

            for (int i = 0; i < names.Count; i++)
            {
                Squad.Add(new SquadMember
                {
                    Name = names[i],
                    Initial = names[i].Length >= 1 ? names[i][..1].ToUpper() : "?",
                    IsActive = i < 2
                });
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
    private async Task OpenProfileAsync() => await Shell.Current.GoToAsync("profile");
}
