using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Services;
using Forge.Views;

namespace Forge.ViewModels;

public partial class SetupViewModel : BaseViewModel
{
    [ObservableProperty] private string serverUrl = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool hasStatus;

    [RelayCommand]
    private void UseDefault()
    {
        Preferences.Set("setup_completed", true);
        Preferences.Remove("pb_server_url");
        GoToLogin();
    }

    [RelayCommand]
    private void SaveAndContinue()
    {
        var url = (ServerUrl ?? "").Trim();

        if (!string.IsNullOrWhiteSpace(url))
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;

            Preferences.Set("pb_server_url", url);
            StatusMessage = $"Server configurato: {url}";
            HasStatus = true;
        }
        else
        {
            Preferences.Remove("pb_server_url");
        }

        Preferences.Set("setup_completed", true);
        GoToLogin();
    }

    private static void GoToLogin()
    {
        var window = App.Current!.Windows[0];
        window.Page = new LoginPage(
            App.Current!.Handler!.MauiContext!.Services.GetService<LoginViewModel>()!);
    }
}
