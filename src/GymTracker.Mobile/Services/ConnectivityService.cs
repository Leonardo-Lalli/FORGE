namespace Forge.Services;

public class ConnectivityService
{
    public bool IsOnline => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        var online = e.NetworkAccess == NetworkAccess.Internet;
        ConnectivityChanged?.Invoke(this, online);
    }
}
