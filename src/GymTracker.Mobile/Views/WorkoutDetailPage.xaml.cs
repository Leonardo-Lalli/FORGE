using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class WorkoutDetailPage : ContentPage
{
    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
