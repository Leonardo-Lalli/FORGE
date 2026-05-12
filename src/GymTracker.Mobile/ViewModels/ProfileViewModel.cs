using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class ProfileWorkout : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string HeartRate { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BorderColor { get; set; } = "#ffb4ab";
}

public partial class ProfileViewModel : BaseViewModel
{
    [ObservableProperty] private string username = "Marcus Kane";
    [ObservableProperty] private string tier = "Elite Tier // Phase IV Conditioning";
    [ObservableProperty] private string totalWorkouts = "342";
    [ObservableProperty] private string energyKcal = "1.2M";
    [ObservableProperty] private string streakDays = "12";
    [ObservableProperty] private string streakLabel = "Day Streak";
    [ObservableProperty] private ObservableCollection<ProfileWorkout> recentWorkouts = new();

    public ProfileViewModel()
    {
        HasData = true;
        LoadMockData();
    }

    private void LoadMockData()
    {
        RecentWorkouts = new ObservableCollection<ProfileWorkout>
        {
            new()
            {
                Title = "Apex Metcon",
                Date = "Today, 06:30 AM",
                Category = "High Intensity",
                CategoryColor = "#ffb4ab",
                Duration = "45m",
                HeartRate = "162 bpm",
                Volume = "12k kg",
                BorderColor = "#ffb4ab",
                Description = "Kettlebell swings (4x15, 24kg), Box jumps (4x10, 24\"), Burpees (AMRAP 5m), Rowing (2km sprint pacing). Focus on explosive hip drive and minimal rest between modalities."
            },
            new()
            {
                Title = "Hypertrophy Core",
                Date = "Yesterday, 18:00 PM",
                Category = "Strength",
                CategoryColor = "#d2d0cf",
                Duration = "65m",
                HeartRate = "128 bpm",
                Volume = "24k kg",
                BorderColor = "#d2d0cf",
                Description = "Deadlifts (5x5, 85% 1RM), Weighted pull-ups (4x8), Bulgarian split squats (4x10/leg), Cable crunches (3x20). Maintained strict tempo 3-1-1-0."
            }
        };
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
