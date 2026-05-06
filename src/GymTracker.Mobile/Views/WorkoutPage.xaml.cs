using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class WorkoutPage : ContentPage
{
    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBellTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("notifications");
    }
}
