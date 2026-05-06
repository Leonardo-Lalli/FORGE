using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnSettingsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }
}
