using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class FriendRequestsPage : ContentPage
{
    public FriendRequestsPage(FriendRequestsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FriendRequestsViewModel vm)
            await vm.LoadRequestsCommand.ExecuteAsync(null);
    }
}
