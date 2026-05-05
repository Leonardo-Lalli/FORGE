using GymTracker.Minimal.ViewModels;

namespace GymTracker.Minimal.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
