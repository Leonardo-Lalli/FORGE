using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private string lastWorkoutSummary = "Nessun allenamento";

    [ObservableProperty]
    private string streakText = "0 giorni";

    [ObservableProperty]
    private string leaderboardPosition = "--";

    [ObservableProperty]
    private string currentWeight = "-- kg";

    [ObservableProperty]
    private string weeklyVolume = "0 kg";

    public DashboardViewModel()
    {
        HasData = false;
        IsEmptyState = true;
    }

    [RelayCommand]
    private async Task OpenNotificationsAsync()
    {
        await Shell.Current.GoToAsync("notifications");
    }
}
