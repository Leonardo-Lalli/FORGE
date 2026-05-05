using GymTracker.Minimal.ViewModels;

namespace GymTracker.Minimal.Views;

public partial class SocialPage : ContentPage
{
    public SocialPage(SocialViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
