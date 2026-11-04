using Forge.ViewModels;

namespace Forge.Views;

public partial class ActiveWorkoutPage : ContentPage
{
    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is ActiveWorkoutViewModel vm) vm.RequestCloseCommand.Execute(null);
        return true;
    }
}
