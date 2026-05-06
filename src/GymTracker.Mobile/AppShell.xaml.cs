using GymTracker.Mobile.Views;

namespace GymTracker.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("activeWorkout", typeof(ActiveWorkoutPage));
    }
}
