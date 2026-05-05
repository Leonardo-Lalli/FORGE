using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class SocialPage : ContentPage
{
    public SocialPage(SocialViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
