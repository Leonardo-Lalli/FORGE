using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Services;

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
    private readonly PocketBaseService pb;

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string tier = string.Empty;
    [ObservableProperty] private string bio = string.Empty;
    [ObservableProperty] private string avatarInitials = "??";
    [ObservableProperty] private string totalWorkouts = "0";
    [ObservableProperty] private string energyKcal = "0";
    [ObservableProperty] private string streakDays = "0";
    [ObservableProperty] private string streakLabel = "Day Streak";
    [ObservableProperty] private ObservableCollection<ProfileWorkout> recentWorkouts = new();

    public ProfileViewModel(PocketBaseService pb)
    {
        this.pb = pb;
        HasData = true;
        LoadData();
    }

    private void LoadData()
    {
        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            var user = pb.CurrentUser;
            Username = string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name;
            Bio = string.IsNullOrWhiteSpace(user.Bio) ? string.Empty : user.Bio;

            if (!string.IsNullOrWhiteSpace(user.Name) && user.Name.Length >= 2)
                AvatarInitials = user.Name[..2].ToUpper();
            else if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Length >= 2)
                AvatarInitials = user.Email[..2].ToUpper();
            else
                AvatarInitials = "??";

            Tier = user.Bio?.Length > 0 ? "Athlete // Active" : "Rookie // Just started";

            LoadMockWorkouts();
        }
        else
        {
            Username = "Offline User";
            Tier = "Demo Mode";
            AvatarInitials = "GT";
            LoadMockWorkouts();
        }
    }

    private void LoadMockWorkouts()
    {
        RecentWorkouts = new ObservableCollection<ProfileWorkout>
        {
            new()
            {
                Title = "Apex Metcon",
                Date = "Today, 06:30 AM",
                Category = "High Intensity",
                Duration = "45m",
                HeartRate = "162 bpm",
                Volume = "12k kg",
                BorderColor = "#ffb4ab",
                Description = "Kettlebell swings (4x15, 24kg), Box jumps (4x10, 24\"), Burpees (AMRAP 5m), Rowing (2km sprint pacing)."
            },
            new()
            {
                Title = "Hypertrophy Core",
                Date = "Yesterday, 18:00 PM",
                Category = "Strength",
                Duration = "65m",
                HeartRate = "128 bpm",
                Volume = "24k kg",
                BorderColor = "#d2d0cf",
                Description = "Deadlifts (5x5, 85% 1RM), Weighted pull-ups (4x8), Bulgarian split squats (4x10/leg), Cable crunches (3x20)."
            }
        };
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
