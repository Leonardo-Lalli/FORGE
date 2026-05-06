using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class SquadMember : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
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
            new() { Name = "Sarah", AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDi8ctfbjj4YouOUb6BxzXbwx5f31w4KZli-JxBv1v2grulQe7-J2_aylwBsvKOU2sVcI09qeLbkOPzkXt0TDiAfRwJsmx9Dhin8H2Y8R41210CywH2MWe4KsQri9HSMzSFgjSUhAE3HVH9u6vbqWn2aMd3kZU8vt6p8dntqC3rWG8uVrPOmQ2q7B1qhGg6cvIFsAWm4f4c1omSnnXJJ1LwEIYWWYhju3GiGTk7pvmY5H9npA_lIxmYuDTgLIK9s_GtwZrfgZRdc7bO", IsActive = true },
            new() { Name = "Mike", AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAJ4gixLSczOBJfKsincuTHu5XInJfiiXSAUDZGCdkFeCRvJBbpF7KDIb6OYOrzh_ryQmnv0FQBB673NVQJAVaRYUH5kIB6bdbfWPF6TX6Ng3n2ZDeKwm9tb4xN6EG9WV0Lm46p3vwig3jpd_x7hI1xaCS53_KJ3Hyb5he04HXhUx-6i7Sq-xVfIF6pf8uURPA8UkQlWNNjn94vxX4e2UdF7ZOqMYgc5PK12s-XUyRECpX5idp0KiJOJr36FwjP-YGyP1Y4-dqN_db2", IsActive = true },
            new() { Name = "Elena", AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCil3EGmm2dEMzcFINqqUrKE-5kwGmKtLlU7vng5NkWb50R1usfuOYuni6NW4fIJ_HIHp5Ka6HNz6Cnt80H1AIUFMbzx5edeB9u29NTBlJBlNImZvBOxBS-cHndDShnZZVRWrfwz9rFk_zkfYp_SzhWb-nJJQvIthoPjICrUHhQQOiGivWtIuuA6U-c1Ub-uhYr5GIgYnmjSlnXCDEBi6B51Gm2XmsFaWHZjeJPLTdZ2ttYtrAFNDt7QFqC0e0_30008s_ZbIGXjCdq", IsActive = false },
            new() { Name = "David", AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCtEojGvHb-QcdsIcZSg7myxr36ianCu6j850pbwAGX2aNT_bmMXbIrJhWohIAPMCFIu5ixIHnQTKp02FevsPH_bBWWjMy8wcfH4y_oKz-77gZEPsZmsofx5gOuZMOgiwJSgBYwPlNEx9emAS3E2xZe7I5RwG7WgeAUDa4BFL3HM_acvrjQto1TKTDMv9--baq01mtwBo_MNlfsPr3LmWOE46Z-wV_1L01bGjCNTCz4QVx0uCG9rpynZzclLJ94-yHCXwKpjLLrXUd", IsActive = false }
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
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "free"
        });
    }

    [RelayCommand]
    private async Task OpenNotificationsAsync()
    {
        await Shell.Current.GoToAsync("notifications");
    }
}
