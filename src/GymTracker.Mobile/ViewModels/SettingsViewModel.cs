using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ThemeService themeService;
    private bool suppressChange;

    [ObservableProperty]
    private bool isDarkMode;

    public SettingsViewModel(ThemeService themeService)
    {
        this.themeService = themeService;
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
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
