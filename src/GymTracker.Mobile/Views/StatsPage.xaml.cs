using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class StatsPage : ContentPage
{
    public StatsPage(StatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
