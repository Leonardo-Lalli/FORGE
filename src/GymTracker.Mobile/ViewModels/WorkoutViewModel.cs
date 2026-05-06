using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Models;

namespace GymTracker.Mobile.ViewModels;

public partial class WorkoutViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<WorkoutPlan> savedPlans = new();

    public WorkoutViewModel()
    {
        IsEmptyState = true;
    }

    [RelayCommand]
    private async Task CreateNewPlanAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "create"
        });
    }

    [RelayCommand]
    private async Task StartEmptyWorkoutAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "free"
        });
    }

    [RelayCommand]
    private async Task OpenSavedPlanAsync(WorkoutPlan plan)
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "saved",
            ["planId"] = plan.Id
        });
    }
}
