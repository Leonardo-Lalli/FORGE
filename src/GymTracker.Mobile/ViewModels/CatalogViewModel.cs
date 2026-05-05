using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class CatalogViewModel : BaseViewModel
{
    [ObservableProperty]
    private string searchText = string.Empty;

    public CatalogViewModel()
    {
        IsEmptyState = true;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return;

        SetLoading();
        await Task.Delay(500);
        SetSuccess(false);
        IsEmptyState = true;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SetLoading();
        await Task.Delay(500);
        SetSuccess(false);
        IsEmptyState = true;
    }
}
