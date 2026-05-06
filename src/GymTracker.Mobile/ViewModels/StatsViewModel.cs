using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GymTracker.Mobile.ViewModels;

public partial class LiftEntry : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
}

public partial class StatsViewModel : BaseViewModel
{
    [ObservableProperty] private string totalHours = "42.5";
    [ObservableProperty] private string totalHoursTrend = "12% vs last month";
    [ObservableProperty] private string calories = "18k";
    [ObservableProperty] private string caloriesTrend = "5% vs last month";
    [ObservableProperty] private string workouts = "24";
    [ObservableProperty] private string workoutsTrend = "Steady pace";
    [ObservableProperty] private string newPRs = "3";
    [ObservableProperty] private string newPRsTrend = "Great week!";
    [ObservableProperty] private string muscleGain = "+2.4kg";
    [ObservableProperty] private ObservableCollection<double> weeklyVolume = new();
    [ObservableProperty] private ObservableCollection<LiftEntry> topLifts = new();

    public StatsViewModel()
    {
        HasData = true;
        LoadMockData();
    }

    private void LoadMockData()
    {
        WeeklyVolume = new ObservableCollection<double> { 0.3, 0.5, 0.8, 0.6, 0.9, 0.4, 0.7 };
        TopLifts = new ObservableCollection<LiftEntry>
        {
            new() { Name = "Deadlift", Label = "1RM", Weight = "185kg" },
            new() { Name = "Squat", Label = "1RM", Weight = "150kg" },
            new() { Name = "Bench Press", Label = "1RM", Weight = "110kg" }
        };
    }
}
