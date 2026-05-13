using GymTracker.Mobile.Views;

namespace GymTracker.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("activeWorkout", typeof(ActiveWorkoutPage));
        Routing.RegisterRoute("notifications", typeof(NotificationsPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("startSession", typeof(StartSessionPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
    }
}
