using Forge.Views;

namespace Forge;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("activeWorkout", typeof(ActiveWorkoutPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("startSession", typeof(StartSessionPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("friendRequests", typeof(FriendRequestsPage));
        Routing.RegisterRoute("workoutDetail", typeof(WorkoutDetailPage));
        Routing.RegisterRoute("achievements", typeof(AchievementsPage));
    }
}
