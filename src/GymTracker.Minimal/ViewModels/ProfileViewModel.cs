using CommunityToolkit.Mvvm.ComponentModel;

namespace GymTracker.Minimal.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    [ObservableProperty]
    private string username = "IronAthlete";

    [ObservableProperty]
    private string email = "atleta@ironrank.fit";

    public ProfileViewModel()
    {
        HasData = true;
    }
}
