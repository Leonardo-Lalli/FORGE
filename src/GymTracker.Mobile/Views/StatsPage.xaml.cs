using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class StatsPage : ContentPage
{
    public StatsPage(StatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatsViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
