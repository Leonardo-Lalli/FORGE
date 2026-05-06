using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutViewModel viewModel;

    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.LoadPlans();
    }

    private async void OnBellTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("notifications");
    }
}
