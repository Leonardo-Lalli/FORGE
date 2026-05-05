using CommunityToolkit.Mvvm.ComponentModel;

namespace GymTracker.Mobile.ViewModels;

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
