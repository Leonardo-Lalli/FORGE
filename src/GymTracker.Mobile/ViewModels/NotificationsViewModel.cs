using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GymTracker.Mobile.ViewModels;

public partial class NotificationItem : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Avatar { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public bool IsFriendRequest { get; set; }
    public bool IsAccepted { get; set; }
}

public partial class NotificationsViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<NotificationItem> notifications = new();

    public NotificationsViewModel()
    {
        HasData = true;
        IsEmptyState = false;
        LoadMockData();
    }

    private void LoadMockData()
    {
        Notifications = new ObservableCollection<NotificationItem>
        {
            new()
            {
                Avatar = "M", Name = "Marco",
                Message = "Vuole aggiungerti agli amici",
                Time = "10m fa", IsFriendRequest = true
            },
            new()
            {
                Avatar = "A", Name = "Andrea",
                Message = "Vuole aggiungerti agli amici",
                Time = "1h fa", IsFriendRequest = true
            },
            new()
            {
                Avatar = "S", Name = "Sofia",
                Message = "Ha commentato il tuo allenamento: \"Grande! Quei 100kg di squat fanno paura\"",
                Time = "2h fa"
            },
            new()
            {
                Avatar = "L", Name = "Luca",
                Message = "Ti ha sfidato: Panca Piana — chi fa più volume questa settimana?",
                Time = "3h fa"
            },
            new()
            {
                Avatar = "M", Name = "Marco",
                Message = "Ha completato un allenamento: 4,200 kg di volume",
                Time = "5h fa"
            },
            new()
            {
                Avatar = "S", Name = "Sofia",
                Message = "Nuovo record personale: 130kg di Stacco!",
                Time = "Ieri"
            }
        };
    }

    [RelayCommand]
    private void AcceptRequest(NotificationItem item)
    {
        item.IsAccepted = true;
        item.Message = "Richiesta accettata! Siete ora amici.";
        item.IsFriendRequest = false;
    }

    [RelayCommand]
    private void DeclineRequest(NotificationItem item)
    {
        Notifications.Remove(item);
    }
}
