using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBellTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("notifications");
    }
}
