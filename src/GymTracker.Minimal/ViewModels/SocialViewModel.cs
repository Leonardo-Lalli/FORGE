using CommunityToolkit.Mvvm.ComponentModel;

namespace GymTracker.Minimal.ViewModels;

public partial class SocialViewModel : BaseViewModel
{
    [ObservableProperty]
    private int friendCount;

    [ObservableProperty]
    private int pendingRequests;

    public SocialViewModel()
    {
        IsEmptyState = true;
    }
}
