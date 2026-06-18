using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Services;
using Forge.Views;

namespace Forge.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ThemeService themeService;
    private readonly PocketBaseService pb;
    private readonly CsvImportService csvImport;
    private readonly CsvExportService csvExport;
    private bool suppressChange;

    public static string LanguageCode => Preferences.Get("exercise_language", "2");

    [ObservableProperty] private bool isDarkMode;
    [ObservableProperty] private string importStatus = string.Empty;
    [ObservableProperty] private bool hasImportStatus;
    [ObservableProperty] private bool isItalian = LanguageCode == "13";
    [ObservableProperty] private bool isEnglish = LanguageCode == "2";

    public SettingsViewModel(ThemeService themeService, PocketBaseService pb,
        CsvImportService csvImport, CsvExportService csvExport)
    {
        this.themeService = themeService;
        this.pb = pb;
        this.csvImport = csvImport;
        this.csvExport = csvExport;
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
    private async Task ImportCsvAsync()
    {
        if (!pb.IsLoggedIn || pb.CurrentUser == null)
        {
            SetImportStatus("Effettua il login prima di importare.");
            return;
        }

        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Seleziona file CSV",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/*", "text/csv", "text/comma-separated-values", "application/octet-stream" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", "public.text" } }
                })
            });

            if (result == null) return;

            using var stream = await result.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var (imported, skipped, errors) = await csvImport.ImportFromCsvAsync(
                content, pb.CurrentUser.Id, pb.CurrentUser.Name);

            var msg = $"Importati {imported} allenamenti. Saltati {skipped}.";
            if (errors.Count > 0)
                msg += $"\nErrori: {string.Join("; ", errors.Take(3))}";
            SetImportStatus(msg);
        }
        catch (Exception ex)
        {
            SetImportStatus($"Errore: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        if (!pb.IsLoggedIn || pb.CurrentUser == null)
        {
            SetImportStatus("Effettua il login prima di esportare.");
            return;
        }

        try
        {
            SetImportStatus("Esportazione in corso...");
            await csvExport.SaveCsvFileAsync(pb.CurrentUser.Id);
            SetImportStatus("Esportazione completata!");
        }
        catch (Exception ex)
        {
            SetImportStatus($"Errore export: {ex.Message}");
        }
    }

    private void SetImportStatus(string msg)
    {
        ImportStatus = msg;
        HasImportStatus = true;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void SetItalian()
    {
        Preferences.Set("exercise_language", "13");
        IsItalian = true;
        IsEnglish = false;
    }

    [RelayCommand]
    private void SetEnglish()
    {
        Preferences.Set("exercise_language", "2");
        IsItalian = false;
        IsEnglish = true;
    }
}
