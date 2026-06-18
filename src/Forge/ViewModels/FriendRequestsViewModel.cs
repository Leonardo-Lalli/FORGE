using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Models.Dto;
using Forge.Services;

namespace Forge.ViewModels;

public partial class FriendRequestItem : ObservableObject
{
    public string RequestId { get; set; } = string.Empty;
    public string FromUserId { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    [ObservableProperty] private bool isAccepted;
}

public partial class LikeNotificationItem : ObservableObject
{
    public string LikerName { get; set; } = string.Empty;
    public string WorkoutName { get; set; } = string.Empty;
}

public partial class FriendRequestsViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private ObservableCollection<FriendRequestItem> requests = new();
    [ObservableProperty] private ObservableCollection<LikeNotificationItem> likeNotifications = new();
    [ObservableProperty] private bool isEmpty;
    [ObservableProperty] private bool hasLikeNotifications;
    [ObservableProperty] private bool hasRequests;

    public FriendRequestsViewModel(PocketBaseService pb)
    {
        this.pb = pb;
    }

    [RelayCommand]
    private async Task LoadRequestsAsync()
    {
        SetLoading();
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

            var likes = await pb.GetLikeNotificationsAsync();
            LikeNotifications.Clear();
            foreach (var (likerName, workoutName, _) in likes)
            {
                LikeNotifications.Add(new LikeNotificationItem
                {
                    LikerName = likerName,
                    WorkoutName = workoutName
                });
            }
            HasLikeNotifications = LikeNotifications.Count > 0;

            IsEmpty = Requests.Count == 0 && LikeNotifications.Count == 0;
            HasData = !IsEmpty;
            HasRequests = Requests.Count > 0;
            HasLikeNotifications = LikeNotifications.Count > 0;
            SetSuccess(HasData);
        }
        catch
        {
            SetError("Errore nel caricamento richieste.");
        }
    }

    [RelayCommand]
    private async Task AcceptAsync(FriendRequestItem item)
    {
        var ok = await pb.AcceptFollowRequestAsync(item.RequestId);
        if (ok)
        {
            item.IsAccepted = true;
            Requests.Remove(item);
            IsEmpty = Requests.Count == 0 && LikeNotifications.Count == 0;
            HasData = !IsEmpty;
            HasRequests = Requests.Count > 0;
        }
    }

    [RelayCommand]
    private async Task RejectAsync(FriendRequestItem item)
    {
        var ok = await pb.RejectFollowRequestAsync(item.RequestId);
        if (ok)
        {
            Requests.Remove(item);
            IsEmpty = Requests.Count == 0 && LikeNotifications.Count == 0;
            HasData = !IsEmpty;
            HasRequests = Requests.Count > 0;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
