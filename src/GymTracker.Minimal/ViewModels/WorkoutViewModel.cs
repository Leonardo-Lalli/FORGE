using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Minimal.ViewModels;

public partial class WorkoutViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool isWorkoutActive;

    [ObservableProperty]
    private string workoutTitle = "Nuovo Allenamento";

    public WorkoutViewModel()
    {
        IsEmptyState = true;
    }

    [RelayCommand]
    private void StartWorkout()
    {
        IsWorkoutActive = true;
        WorkoutTitle = "Allenamento in corso...";
        HasData = true;
        IsEmptyState = false;
    }
}
