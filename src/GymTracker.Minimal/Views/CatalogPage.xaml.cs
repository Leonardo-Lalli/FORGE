using GymTracker.Minimal.ViewModels;

namespace GymTracker.Minimal.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage(CatalogViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
