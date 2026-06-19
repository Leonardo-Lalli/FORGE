using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Models.Dto;
using Forge.Services;

namespace Forge.ViewModels;

public partial class FeedPost : ObservableObject
{
    public string WorkoutId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ExercisesList { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    [ObservableProperty] private int likes;
    [ObservableProperty] private bool isLiked;
    [ObservableProperty] private string heartIcon = "\u2661";
    [ObservableProperty] private string avatarUrl = string.Empty;
    [ObservableProperty] private ImageSource? avatarSource;
    [ObservableProperty] private bool hasAvatar;
}

public partial class UserSearchResult : ObservableObject
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Initial { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;

    [ObservableProperty] private bool isFollowing;
    [ObservableProperty] private string followLabel = "Follow";
    [ObservableProperty] private ImageSource? avatarSource;
    [ObservableProperty] private bool hasAvatar;
}

public partial class FeedViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private ObservableCollection<FeedPost> posts = new();
    [ObservableProperty] private ObservableCollection<UserSearchResult> searchResults = new();
    [ObservableProperty] private string searchQuery = string.Empty;

    partial void OnSearchQueryChanged(string value)
    {
        if (value.Length >= 2)
            _ = SearchUsersAsyncDebounced(value).ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[Feed SrchDeb] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private CancellationTokenSource? searchCts;

    private async Task SearchUsersAsyncDebounced(string query)
    {
        searchCts?.Cancel();
        searchCts = new CancellationTokenSource();
        var token = searchCts.Token;
        try
        {
            await Task.Delay(400, token);
            if (!token.IsCancellationRequested)
                MainThread.BeginInvokeOnMainThread(() => _ = SearchUsersCoreAsync(token).ContinueWith(t => { if (t.IsFaulted) System.Diagnostics.Debug.WriteLine($"[Feed Srch] ex: {t.Exception?.InnerException?.Message}"); }, TaskContinuationOptions.OnlyOnFaulted));
        }
        catch (TaskCanceledException) { /* debounce cancelled, expected */ }
    }

    private async Task SearchUsersCoreAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2) return;
        if (!pb.IsLoggedIn) return;

        IsSearching = true;
        try
        {
            if (ct.IsCancellationRequested) return;
            var followingIds = await pb.GetFollowingUserIdsAsync();
            if (ct.IsCancellationRequested) return;
            var results = await pb.SearchUsersAsync(SearchQuery);
            System.Diagnostics.Debug.WriteLine($"[FeedSearch] query='{SearchQuery}' results={results.Count}");
            SearchResults.Clear();
            foreach (var user in results)
            {
                if (ct.IsCancellationRequested) return;
                if (user.Id == pb.CurrentUser?.Id) continue;
                System.Diagnostics.Debug.WriteLine($"[FeedSearch] user={user.Name} id={user.Id}");
                var isFollowing = followingIds.Contains(user.Id);
                var avatarUrl = string.IsNullOrWhiteSpace(user.Avatar) ? "" : pb.GetFileUrl(user.CollectionId, user.Id, user.Avatar);
                SearchResults.Add(new UserSearchResult
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Initial = (user.Name.Length > 0 ? user.Name[..1].ToUpper() : "?"),
                    IsFollowing = isFollowing,
                    FollowLabel = isFollowing ? "Following" : "Follow",
                    AvatarUrl = avatarUrl,
                    AvatarSource = !string.IsNullOrWhiteSpace(avatarUrl)
                        ? ImageSource.FromUri(new Uri(avatarUrl)) : null,
                    HasAvatar = !string.IsNullOrWhiteSpace(user.Avatar)
                });
            }
            HasSearchResults = SearchResults.Count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeedSearch] ex={ex.Message}");
            ErrorMessage = "Errore ricerca utenti.";
        }
        finally { IsSearching = false; }
    }
    [ObservableProperty] private bool isSearching;
    [ObservableProperty] private bool isFeedBusy;
    [ObservableProperty] private bool hasFeed;
    [ObservableProperty] private bool hasSearchResults;
    [ObservableProperty] private string userInitials = "GT";
    [ObservableProperty] private string userAvatarUrl = string.Empty;
    [ObservableProperty] private ImageSource? userAvatarSource;
    [ObservableProperty] private bool hasUserAvatar;

    public FeedViewModel(PocketBaseService pb)
    {
        this.pb = pb;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await LoadFeedAsync());
        });
    }

    [RelayCommand]
    private async Task LoadFeedAsync()
    {
        LoadUserInfo();
        if (!pb.IsLoggedIn) return;
        IsFeedBusy = true;
        try
        {
            var workouts = await pb.GetFollowedWorkoutsAsync();
            Posts.Clear();
            foreach (var w in workouts)
            {
                var likedBy = w.LikedBy ?? new();
                var timeAgo = "";
                if (DateTime.TryParse(w.Date, out var dt))
                {
                    var span = DateTime.UtcNow - dt.ToUniversalTime();
                    timeAgo = span.TotalHours < 1 ? $"{span.Minutes}m ago"
                        : span.TotalDays < 1 ? $"{span.Hours}h ago"
                        : $"{span.Days}d ago";
                }

                Posts.Add(new FeedPost
                {
                    WorkoutId = w.Id,
                    UserName = w.UserName,
                    Initial = (w.UserName.Length > 0 ? w.UserName[..1].ToUpper() : "?"),
                    TimeAgo = timeAgo,
                    Title = w.Name,
                    ExercisesList = string.Join(", ", w.Exercises ?? new()),
                    Volume = $"{w.Volume:0.#} kg",
                    Duration = $"{w.Duration} min",
                    Likes = w.Likes,
                    IsLiked = likedBy.Contains(pb.CurrentUser?.Id ?? ""),
                    HeartIcon = likedBy.Contains(pb.CurrentUser?.Id ?? "") ? "\u2665" : "\u2661",
                    AvatarUrl = w.AvatarUrl ?? "",
                    AvatarSource = !string.IsNullOrWhiteSpace(w.AvatarUrl)
                        ? ImageSource.FromUri(new Uri(w.AvatarUrl)) : null,
                    HasAvatar = !string.IsNullOrWhiteSpace(w.AvatarUrl)
                });
            }
            HasFeed = Posts.Count > 0;
            HasData = true;
        }
        catch
        {
            ErrorMessage = "Errore caricamento feed.";
        }
        finally { IsFeedBusy = false; }
    }

    [RelayCommand]
    private async Task OpenWorkoutDetailAsync(FeedPost post)
    {
        if (string.IsNullOrWhiteSpace(post?.WorkoutId)) return;
        await Shell.Current.GoToAsync($"workoutDetail?workoutId={Uri.EscapeDataString(post.WorkoutId)}&source=pocketbase");
    }

    [RelayCommand]
    private async Task SearchUsersAsync()
    {
        await SearchUsersCoreAsync(CancellationToken.None);
    }

    [RelayCommand]
    private async Task FollowUserAsync(UserSearchResult user)
    {
        if (user.IsFollowing) return;
        var ok = await pb.SendFollowRequestAsync(user.UserId);
        if (ok)
        {
            user.FollowLabel = "Requested";
            user.IsFollowing = true;
        }
    }

    [RelayCommand]
    private async Task LikeWorkoutAsync(FeedPost post)
    {
        if (!pb.IsLoggedIn) return;
        if (post.IsLiked)
        {
            var (ok, _) = await pb.UnlikeWorkoutAsync(post.WorkoutId);
            if (ok)
            {
                post.IsLiked = false;
                post.HeartIcon = "♡";
                post.Likes = Math.Max(0, post.Likes - 1);
            }
        }
        else
        {
            var (ok, _) = await pb.LikeWorkoutAsync(post.WorkoutId);
            if (ok)
            {
                post.IsLiked = true;
                post.HeartIcon = "♥";
                post.Likes++;
            }
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync() => await Shell.Current.GoToAsync("settings");

    [RelayCommand]
    private async Task OpenFriendRequestsAsync() => await Shell.Current.GoToAsync("friendRequests");

    [RelayCommand]
    private async Task OpenProfileAsync() => await Shell.Current.GoToAsync("profile");

    private void LoadUserInfo()
    {
        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            var u = pb.CurrentUser;
            UserInitials = (u.Name?.Length >= 2) ? u.Name[..2].ToUpper() : (u.Email?.Length >= 2 ? u.Email[..2].ToUpper() : "GT");
            if (!string.IsNullOrWhiteSpace(u.Avatar))
            {
                UserAvatarUrl = pb.GetFileUrl(u.CollectionId, u.Id, u.Avatar);
                UserAvatarSource = ImageSource.FromUri(new Uri(UserAvatarUrl));
                HasUserAvatar = true;
            }
            else { UserAvatarSource = null; HasUserAvatar = false; }
        }
    }
}
