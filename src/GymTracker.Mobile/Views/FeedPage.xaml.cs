using Forge.ViewModels;

namespace Forge.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FeedViewModel vm)
            await vm.LoadFeedCommand.ExecuteAsync(null);
    }
}
