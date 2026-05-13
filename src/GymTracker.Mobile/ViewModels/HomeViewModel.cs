using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class SquadMember : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public partial class Achievement : ObservableObject
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public double Progress { get; set; }
    public string ProgressText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public partial class HomeViewModel : BaseViewModel
{
    [ObservableProperty] private string streakCount = "12";
    [ObservableProperty] private string streakLabel = "WEEKS";
    [ObservableProperty] private string streakDescription = "You are crushing it. Keep the momentum going for another week.";
    [ObservableProperty] private string nextWorkout = "Upper Body";
    [ObservableProperty] private string workoutDuration = "45 Min · High Intensity";
    [ObservableProperty] private double hydrationProgress = 0.5;
    [ObservableProperty] private string hydrationText = "1.5 / 3L";
    [ObservableProperty] private ObservableCollection<SquadMember> squad = new();
    [ObservableProperty] private ObservableCollection<Achievement> achievements = new();

    public HomeViewModel()
    {
        HasData = true;
        LoadMockData();
    }

    private void LoadMockData()
    {
        Squad = new ObservableCollection<SquadMember>
        {
            new() { Name = "Sarah", Initial = "S", IsActive = true },
            new() { Name = "Mike", Initial = "M", IsActive = true },
            new() { Name = "Elena", Initial = "E", IsActive = false },
            new() { Name = "David", Initial = "D", IsActive = false }
        };

        Achievements = new ObservableCollection<Achievement>
        {
            new() { Icon = "⬡", Title = "Power Lifter", Progress = 0.8, ProgressText = "80%", IsActive = true },
            new() { Icon = "○", Title = "Consistency King", Progress = 0.45, ProgressText = "45%", IsActive = false }
        };
    }

    [RelayCommand]
    private async Task StartWorkoutAsync()
    {
        await Shell.Current.GoToAsync("startSession");
    }

    [RelayCommand]
    private async Task OpenNotificationsAsync()
    {
        await Shell.Current.GoToAsync("notifications");
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("settings");
    }

    [RelayCommand]
    private async Task OpenProfileAsync()
    {
        await Shell.Current.GoToAsync("profile");
    }
}
