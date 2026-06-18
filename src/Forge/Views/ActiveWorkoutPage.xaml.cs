using Forge.ViewModels;

namespace Forge.Views;

public partial class ActiveWorkoutPage : ContentPage
{
    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
