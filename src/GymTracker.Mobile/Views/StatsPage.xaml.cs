using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class StatsPage : ContentPage
{
    public StatsPage(StatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnSettingsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }
}
