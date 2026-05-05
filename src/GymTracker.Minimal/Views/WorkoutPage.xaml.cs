using GymTracker.Minimal.ViewModels;

namespace GymTracker.Minimal.Views;

public partial class WorkoutPage : ContentPage
{
    public WorkoutPage(WorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
