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

        // Pre-calculated bar heights in pixels (160px container max)
        public double BarHeight0 => 48;
        public double BarHeight1 => 80;
        public double BarHeight2 => 128;
        public double BarHeight3 => 96;
        public double BarHeight4 => 144;
        public double BarHeight5 => 64;
        public double BarHeight6 => 112;

        public StatsViewModel()
        {
            HasData = true;
            LoadMockData();
        }

        private void LoadMockData()
        {
            TopLifts = new ObservableCollection<LiftEntry>
        {
            new() { Name = "Deadlift", Label = "1RM", Weight = "185kg" },
            new() { Name = "Squat", Label = "1RM", Weight = "150kg" },
            new() { Name = "Bench Press", Label = "1RM", Weight = "110kg" }
        };
    }
}
