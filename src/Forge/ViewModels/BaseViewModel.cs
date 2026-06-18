using CommunityToolkit.Mvvm.ComponentModel;

namespace Forge.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasData;

    [ObservableProperty]
    private bool isEmptyState;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    protected void SetLoading()
    {
        IsBusy = true;
        ErrorMessage = null;
        HasData = false;
        IsEmptyState = false;
    }

    protected void SetSuccess(bool hasData)
    {
        IsBusy = false;
        ErrorMessage = null;
        HasData = hasData;
        IsEmptyState = !hasData;
    }

    protected void SetError(string message)
    {
        IsBusy = false;
        ErrorMessage = message;
        HasData = false;
        IsEmptyState = false;
    }
}
