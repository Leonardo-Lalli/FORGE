using GymTracker.Minimal.ViewModels;

namespace GymTracker.Minimal.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
