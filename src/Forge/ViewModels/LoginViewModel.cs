using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Services;

namespace Forge.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string passwordConfirm = string.Empty;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private bool isRegistering;

    public LoginViewModel(PocketBaseService pb)
    {
        this.pb = pb;
        HasData = true;
        _ = TryAutoLoginAsync().ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
                System.Diagnostics.Debug.WriteLine($"[Login AutoLogin] ex: {t.Exception.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private async Task TryAutoLoginAsync()
    {
        await Task.Delay(1000);
        if (IsBusy || IsRegistering) return;
        var success = await pb.TryAutoLoginAsync();
        if (success)
        {
            var window = App.Current!.Windows[0];
            window.Page = new AppShell();
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Inserisci email e password.");
            return;
        }

        SetLoading();
        var (success, error) = await pb.LoginAsync(Email.Trim(), Password);

        if (success)
        {
            SetSuccess(true);
            var window = App.Current!.Windows[0];
            window.Page = new AppShell();
        }
        else
        {
            SetError(error);
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Inserisci email, password e nome.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Name)) { SetError("Inserisci il nome atleta."); return; }

        if (Password != PasswordConfirm)
        {
            SetError("Le password non coincidono.");
            return;
        }

        SetLoading();
        var (success, error) = await pb.RegisterAsync(Email.Trim(), Password, Name.Trim());

        if (success)
        {
            SetSuccess(true);
            var window = App.Current!.Windows[0];
            window.Page = new AppShell();
        }
        else
        {
            SetError(error);
        }
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegistering = !IsRegistering;
        ErrorMessage = null;
    }

    [RelayCommand]
    private async Task OpenServerConfigAsync()
    {
        var current = Preferences.Get("pb_server_url", string.Empty);
        var page = Application.Current?.Windows[0].Page;
        if (page == null) return;

        var result = await page.DisplayPromptAsync(
            "Server PocketBase",
            "URL del tuo server. Lascia vuoto per usare FORGE pubblico.",
            "Salva",
            "Annulla",
            placeholder: "https://tuo-server.duckdns.org",
            initialValue: current);

        if (result == null) return;

        var url = result.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            Preferences.Remove("pb_server_url");
            pb.InvalidateClient();
            return;
        }

        if (!ServerAddress.TryNormalize(url, out url))
        { SetError("Inserisci un URL HTTP o HTTPS valido."); return; }

        Preferences.Set("pb_server_url", url);
        pb.InvalidateClient();
        ErrorMessage = null;
    }
}
