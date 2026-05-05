using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage(CatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
