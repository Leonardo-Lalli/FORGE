using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Models.Dto;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class FriendRequestItem : ObservableObject
{
    public string RequestId { get; set; } = string.Empty;
    public string FromUserId { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    [ObservableProperty] private bool isAccepted;
}

public partial class FriendRequestsViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private ObservableCollection<FriendRequestItem> requests = new();
    [ObservableProperty] private bool isEmpty;
    [ObservableProperty] private bool isBusy;

    public FriendRequestsViewModel(PocketBaseService pb)
    {
        this.pb = pb;
    }

    [RelayCommand]
    private async Task LoadRequestsAsync()
    {
        IsBusy = true;
        try
        {
            var pending = await pb.GetPendingRequestsAsync();
            Requests.Clear();
            foreach (var r in pending)
            {
                Requests.Add(new FriendRequestItem
                {
                    RequestId = r.Id,
                    FromUserId = r.FromUser,
                    FromName = r.FromName ?? "Unknown"
                });
            }
            IsEmpty = Requests.Count == 0;
            HasData = !IsEmpty;
        }
        catch
        {
            ErrorMessage = "Errore nel caricamento richieste.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AcceptAsync(FriendRequestItem item)
    {
        var ok = await pb.AcceptFollowRequestAsync(item.RequestId);
        if (ok)
        {
            item.IsAccepted = true;
            Requests.Remove(item);
            IsEmpty = Requests.Count == 0;
            HasData = !IsEmpty;
        }
    }

    [RelayCommand]
    private async Task RejectAsync(FriendRequestItem item)
    {
        var ok = await pb.RejectFollowRequestAsync(item.RequestId);
        if (ok)
        {
            Requests.Remove(item);
            IsEmpty = Requests.Count == 0;
            HasData = !IsEmpty;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
