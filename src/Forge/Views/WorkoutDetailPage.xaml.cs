using Forge.ViewModels;

namespace Forge.Views;

public partial class WorkoutDetailPage : ContentPage
{
    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
