using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class AchievementsPage : ContentPage
{
    public AchievementsPage(AchievementsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AchievementsViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
