using GymTracker.Mobile.ViewModels;

namespace GymTracker.Mobile.Views;

public partial class StartSessionPage : ContentPage
{
    public StartSessionPage(StartSessionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StartSessionViewModel vm)
            _ = vm.LoadProtocolsAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    System.Diagnostics.Debug.WriteLine($"[StartSession OnAppearing] ex: {t.Exception?.InnerException?.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
