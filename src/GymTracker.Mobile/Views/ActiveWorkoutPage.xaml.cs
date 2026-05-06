using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class ActiveWorkoutPage : ContentPage
{
    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
