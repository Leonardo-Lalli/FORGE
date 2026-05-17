using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
