using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
