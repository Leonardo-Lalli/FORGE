namespace Forge.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(400);

        await Task.WhenAll(
            TitleLabel.FadeTo(1, 600, Easing.CubicOut),
            SubtitleLabel.FadeTo(1, 600, Easing.CubicOut)
        );

        await Task.Delay(1400);
    }
}
