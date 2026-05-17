using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
