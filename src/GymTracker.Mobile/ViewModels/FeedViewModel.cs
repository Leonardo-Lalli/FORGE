using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GymTracker.Mobile.ViewModels;

public partial class FeedPost : ObservableObject
{
    public string UserName { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool HasImage { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }

    // Workout data (3 columns)
    public string DataCol1Label { get; set; } = string.Empty;
    public string DataCol1Value { get; set; } = string.Empty;
    public string DataCol2Label { get; set; } = string.Empty;
    public string DataCol2Value { get; set; } = string.Empty;
    public string DataCol3Label { get; set; } = string.Empty;
    public string DataCol3Value { get; set; } = string.Empty;

    // Volume/Max cards (2-column)
    public string Card1Label { get; set; } = string.Empty;
    public string Card1Value { get; set; } = string.Empty;
    public string Card2Label { get; set; } = string.Empty;
    public string Card2Value { get; set; } = string.Empty;
    public bool HasDataCards { get; set; }
}

public partial class FeedViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<FeedPost> posts = new();

    public FeedViewModel()
    {
        HasData = true;
        LoadMockData();
    }

    private void LoadMockData()
    {
        Posts = new ObservableCollection<FeedPost>
        {
            new()
            {
                UserName = "Sarah Jenkins",
                Initial = "S",
                TimeAgo = "2 hours ago",
                Category = "High Intensity",
                Title = "Morning Threshold Run",
                DataCol1Label = "DURATION", DataCol1Value = "45:20",
                DataCol2Label = "DISTANCE", DataCol2Value = "8.5 km",
                DataCol3Label = "AVG HR", DataCol3Value = "168 bpm",
                ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuB84c2IsvVc90jKf1NcTI1hMYgA4n2vdKEbfnukZPe9HENk8D86shNk1BuptkU6jnyKpRqojw703Ia9Rk4u9oPpFXCFpZ_vuJOMPGhkUgPD5R92ySZ77dvx7jVlx-lVmpdJnXBqQmRGc6S0qPQtNwS5mm7HriDt7O7M3jK4MsIgE8ZR5pdUYEjfla34_udWoFA0a0jsXtYWUpeTZJkbO6zH77raAIuG8wy55bKlFd4WqfMgaSP3G-trwpPOMSKQdryCQR44kJlDlCw",
                HasImage = true,
                Likes = 24, Comments = 5,
                HasDataCards = false
            },
            new()
            {
                UserName = "Marcus Chen",
                Initial = "M",
                TimeAgo = "5 hours ago",
                Category = "Strength",
                Title = "Heavy Deadlift Session",
                Description = "Finally hit the 400lb club. Felt solid off the floor.",
                Card1Label = "VOLUME", Card1Value = "8,500 lbs",
                Card2Label = "MAX EFFORT", Card2Value = "405 lbs",
                HasDataCards = true,
                HasImage = false,
                Likes = 42, Comments = 12
            }
        };
    }
}
