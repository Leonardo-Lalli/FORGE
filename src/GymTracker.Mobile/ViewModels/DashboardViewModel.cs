using CommunityToolkit.Mvvm.ComponentModel;

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
        HasData = true;
        IsEmptyState = false;
    }
}
