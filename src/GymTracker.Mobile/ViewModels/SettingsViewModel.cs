using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Services;
using GymTracker.Mobile.Views;

namespace GymTracker.Mobile.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ThemeService themeService;
    private readonly PocketBaseService pb;
    private bool suppressChange;

    [ObservableProperty]
    private bool isDarkMode;

    public SettingsViewModel(ThemeService themeService, PocketBaseService pb)
    {
        this.themeService = themeService;
        this.pb = pb;
        suppressChange = true;
        IsDarkMode = themeService.IsDarkMode;
        suppressChange = false;
        HasData = true;
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        if (suppressChange) return;
        themeService.IsDarkMode = value;
    }

    [RelayCommand]
    private void Logout()
    {
        pb.Logout();
        var window = App.Current!.Windows[0];
        window.Page = new LoginPage(
            App.Current!.Handler!.MauiContext!.Services.GetService<LoginViewModel>()!);
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
