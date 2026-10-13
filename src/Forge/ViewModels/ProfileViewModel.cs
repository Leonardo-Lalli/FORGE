using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Forge.Messages;
using Forge.Models;
using Forge.Services;

namespace Forge.ViewModels;

public partial class ProfileWorkout : ObservableObject
{
    public string WorkoutId { get; set; } = string.Empty;
    public string Source { get; set; } = "pocketbase";
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Exercises { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;
    public int Likes { get; set; }
    public string BorderColor { get; set; } = "#ffb4ab";
    public string Notes { get; set; } = string.Empty;
    public bool HasNotes { get; set; }
    public string FirstPhoto { get; set; } = string.Empty;
    public bool HasPhotos { get; set; }
}

[QueryProperty(nameof(ViewedUserId), "userId")]
public partial class ProfileViewModel : BaseViewModel
{
    [ObservableProperty] private string viewedUserId = "";
    [ObservableProperty] private bool isOwnProfile = true;
    [ObservableProperty] private int followersCount;
    [ObservableProperty] private int followingCount;
    [ObservableProperty] private bool isConnectionsVisible;
    [ObservableProperty] private string connectionsTitle = "";
    [ObservableProperty] private int visibilityIndex;
    [ObservableProperty] private int connectionsVisibilityIndex;
    [ObservableProperty] private bool canViewConnections;
    [ObservableProperty] private bool isConnectionsLoading;
    [ObservableProperty] private string connectionsMessage = "Nessun utente nell’elenco.";
    [ObservableProperty] private string connectionsError = "";
    [ObservableProperty] private string profileFollowLabel = "Segui";
    [ObservableProperty] private bool isUnfollowConfirmationVisible;
    [ObservableProperty] private string unfollowName = "";
    [ObservableProperty] private bool isFollowBusy;
    private UserSearchResult? pendingUnfollow;
    private List<Models.Dto.SocialGraphRecord> ownRelationships = [];
    public IReadOnlyList<string> VisibilityChoices { get; } = new[] { "Pubblico", "Privato", "Solo amici" };
    public ObservableCollection<UserSearchResult> Connections { get; } = new();
    public ObservableCollection<BadgeChoice> BadgeChoices { get; } = new();
    private Models.Dto.PocketBaseUserRecord? displayedUser;
    private List<Models.Dto.SocialGraphRecord> relationships = [];
    private readonly PocketBaseService pb;
    private readonly AchievementService achievementService;
    private readonly DatabaseService db;
    private List<Models.Dto.LoggedWorkoutRecord> loadedWorkouts = [];

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string tier = string.Empty;
    [ObservableProperty] private string bio = string.Empty;
    [ObservableProperty] private string avatarInitials = "??";
    [ObservableProperty] private string avatarUrl = string.Empty;
    [ObservableProperty] private ImageSource? avatarSource;
    [ObservableProperty] private bool hasAvatar;
    [ObservableProperty] private bool isLoggedIn;
    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private string editName = string.Empty;
    [ObservableProperty] private string editBio = string.Empty;
    [ObservableProperty] private string totalWorkouts = "0";
    [ObservableProperty] private string streakDays = "0";
    [ObservableProperty] private string streakLabel = "Week Streak";
    [ObservableProperty] private string totalVolume = "0";
    [ObservableProperty] private string totalLikesReceived = "0";
    [ObservableProperty] private string workoutLoadError = string.Empty;
    [ObservableProperty] private bool hasWorkoutError;
    [ObservableProperty] private ObservableCollection<ProfileWorkout> recentWorkouts = new();
    [ObservableProperty] private ObservableCollection<string> unlockedBadges = new();
    [ObservableProperty] private bool hasUnlockedBadges;
    [ObservableProperty] private string achievementSummary = string.Empty;

    public ProfileViewModel(PocketBaseService pb, AchievementService achievementService, DatabaseService db)
    {
        this.pb = pb;
        this.achievementService = achievementService;
        this.db = db;
        HasData = true;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => { if (string.IsNullOrEmpty(ViewedUserId) || ViewedUserId == pb.CurrentUser?.Id) await LoadRealWorkouts(true); });
        });
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoggedIn = pb.IsLoggedIn;

        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            try
            {
            IsOwnProfile = string.IsNullOrEmpty(ViewedUserId) || ViewedUserId == pb.CurrentUser.Id;
            if (IsOwnProfile) { await LoadRealWorkouts(true); await pb.RefreshUserAsync(); }
            var user = IsOwnProfile ? pb.CurrentUser : await pb.GetUserByIdAsync(ViewedUserId);
            if (user == null) { SetError("Profilo non disponibile."); return; }
            displayedUser = user;
            Username = string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name;
            Bio = string.IsNullOrWhiteSpace(user.Bio) ? "" : user.Bio;

            if (!string.IsNullOrWhiteSpace(user.Name) && user.Name.Length >= 2)
                AvatarInitials = user.Name[..2].ToUpper();
            else if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Length >= 2)
                AvatarInitials = user.Email[..2].ToUpper();
            else
                AvatarInitials = "??";

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                AvatarSource = await pb.DownloadAvatarAsync(user.CollectionId, user.Id, user.Avatar);
                HasAvatar = AvatarSource != null;
                System.Diagnostics.Debug.WriteLine($"[Profile] Avatar downloaded: {HasAvatar}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Profile] No avatar field on user record");
                AvatarSource = null;
                HasAvatar = false;
            }

            Tier = "FORGE ATHLETE";

            await LoadRealWorkouts();
            await LoadAchievementBadges();
            var connections = await pb.GetVisibleConnectionsAsync(user.Id);
            CanViewConnections = connections.Allowed;
            relationships = connections.Items;
            FollowersCount = relationships.Where(r => r.ToUser == user.Id).Select(r => r.FromUser).Distinct().Count();
            FollowingCount = relationships.Where(r => r.FromUser == user.Id).Select(r => r.ToUser).Distinct().Count();
            ownRelationships = await pb.GetOwnRelationshipsAsync();
            ProfileFollowLabel = FollowLabelFor(user.Id);
            }
            catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
            { SetError("Impossibile caricare il profilo. Riprova."); }
        }
        else
        {
            Username = "Offline User";
            Tier = "Demo Mode";
            AvatarInitials = "GT";
            HasAvatar = false;
        }
    }

    private async Task LoadRealWorkouts(bool localOnly = false)
    {
        WorkoutLoadError = string.Empty;
        HasWorkoutError = false;
        try
        {
            System.Diagnostics.Debug.WriteLine("[Profile] LoadRealWorkouts starting...");
            var workouts = IsOwnProfile ? await WorkoutHistory.LoadOwnAsync(pb, db, localOnly, loadedWorkouts) : await pb.GetUserWorkoutsAsync(displayedUser?.Id ?? "");
            if (IsOwnProfile && !localOnly) workouts = await WorkoutHistory.LoadOwnAsync(pb, db, true, workouts);
            loadedWorkouts = workouts;
            System.Diagnostics.Debug.WriteLine($"[Profile] GetMyWorkoutsAsync returned {workouts.Count} items");
            TotalWorkouts = workouts.Count.ToString();

            double totalVol = 0;
            int totalLikes = 0;
            RecentWorkouts.Clear();

            var colors = new[] { "#ffb4ab", "#00E5FF", "#CCFF00", "#a5d6ff" };
            int ci = 0;

            foreach (var w in workouts)
            {
                totalVol += w.Volume;
                totalLikes += w.Likes;

                var dateStr = "";
                if (DateTime.TryParse(w.Date, out var dt))
                    dateStr = dt.ToLocalTime().ToString("ddd dd MMM, HH:mm");

                RecentWorkouts.Add(new ProfileWorkout
                {
                    WorkoutId = w.Id,
                    Source = w.IsLocal ? "local" : "pocketbase",
                    Title = w.Name,
                    Date = dateStr,
                    Exercises = string.Join(", ", w.Exercises ?? new()),
                    Duration = $"{w.Duration} min",
                    Volume = $"{w.Volume:0.#} kg",
                    Likes = w.Likes,
                    BorderColor = colors[ci % colors.Length],
                    Notes = w.Notes ?? "",
                    HasNotes = !string.IsNullOrWhiteSpace(w.Notes),
                    FirstPhoto = (await pb.GetWorkoutPhotoUrlsAsync(w)).FirstOrDefault() ?? "",
                    HasPhotos = w.Photos.Count > 0
                });
                ci++;
            }

            TotalVolume = $"{totalVol:0.#} kg";
            TotalLikesReceived = totalLikes.ToString();

            if (workouts.Count == 0)
            {
                WorkoutLoadError = "Nessun allenamento salvato. Inizia un allenamento!";
                HasWorkoutError = true;
            }

            await CalculateStreakAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Profile] LoadRealWorkouts ex: {ex}");
            TotalWorkouts = "0";
            TotalVolume = "0 kg";
            WorkoutLoadError = $"Errore caricamento: {ex.Message}";
            HasWorkoutError = true;
        }
    }

    private async Task CalculateStreakAsync()
    {
        try
        {
            var workouts = loadedWorkouts;
            if (workouts.Count == 0) { StreakDays = "0"; StreakLabel = "Week Streak"; return; }

            var dates = workouts
                .Select(w => { DateTime.TryParse(w.Date, out var d); return d; })
                .Where(d => d != default)
                .Select(d => d.ToLocalTime().Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            if (dates.Count == 0) { StreakDays = "0"; StreakLabel = "Week Streak"; return; }

            var mostRecent = dates.First();
            var daysSinceLastWorkout = (DateTime.Now.Date - mostRecent).Days;
            if (daysSinceLastWorkout > 7) { StreakDays = "0"; StreakLabel = "Week Streak"; return; }

            var weekStart = mostRecent.AddDays(-(int)mostRecent.DayOfWeek + 1);
            if (mostRecent.DayOfWeek == DayOfWeek.Sunday)
                weekStart = mostRecent.AddDays(-6);

            int streak = 0;
            while (true)
            {
                var weekEnd = weekStart.AddDays(6);
                if (!dates.Any(d => d >= weekStart && d <= weekEnd)) break;
                streak++;
                weekStart = weekStart.AddDays(-7);
            }

            StreakDays = streak.ToString();
            StreakLabel = streak == 1 ? "Week Streak" : "Weeks Streak";
        }
        catch
        {
            StreakDays = "0";
        }
    }

    [RelayCommand]
    private void EditProfile()
    {
        if (!IsOwnProfile) return;
        EditName = Username;
        EditBio = Bio;
        VisibilityIndex = displayedUser?.WorkoutVisibility switch { "public" => 0, "friends" => 2, _ => 1 };
        ConnectionsVisibilityIndex = displayedUser?.ConnectionsVisibility switch { "public" => 0, "friends" => 2, _ => 1 };
        foreach (var badge in BadgeChoices) badge.IsSelected = displayedUser?.FeaturedAchievements.Contains(badge.Id) == true;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task OpenWorkoutDetailAsync(ProfileWorkout workout)
    {
        if (string.IsNullOrWhiteSpace(workout?.WorkoutId)) return;
        await Shell.Current.GoToAsync($"workoutDetail?workoutId={Uri.EscapeDataString(workout.WorkoutId)}&source={workout.Source}");
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        ErrorMessage = null;
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(EditName))
        {
            SetError("Il nome non puo essere vuoto.");
            return;
        }

        SetLoading();
        var selected = BadgeChoices.Where(b => b.IsSelected).Select(b => b.Id).ToList();

        try
        {
        var (success, error) = await pb.UpdateProfileOptionsAsync(EditName.Trim(), EditBio, VisibilityIndex switch { 1 => "private", 2 => "friends", _ => "public" }, selected, ConnectionsVisibilityIndex switch { 1 => "private", 2 => "friends", _ => "public" });

        if (success)
        {
            Username = EditName;
            Bio = EditBio;
            IsEditing = false;
            SetSuccess(true);
            displayedUser = pb.CurrentUser;
            await LoadAchievementBadges();
        }
        else
        {
            SetError(error);
        }
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
        { SetError("Salvataggio non riuscito. Controlla la connessione."); }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ChangeAvatarAsync()
    {
        if (!pb.IsLoggedIn || !IsOwnProfile) return;

        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Scegli foto profilo",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) return;

            using var stream = await result.OpenReadAsync();
            var ok = await pb.UploadAvatarAsync(stream, result.FileName);
            System.Diagnostics.Debug.WriteLine($"[Profile] UploadAvatar result: {ok}");
            if (ok)
            {
                await pb.RefreshUserAsync();
                var user = pb.CurrentUser;
                if (user != null && !string.IsNullOrWhiteSpace(user.Avatar))
                {
                    AvatarSource = await pb.DownloadAvatarAsync(user.CollectionId, user.Id, user.Avatar);
                    HasAvatar = AvatarSource != null;
                    System.Diagnostics.Debug.WriteLine($"[Profile] New avatar downloaded: {HasAvatar}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Profile] Avatar still empty after upload/refresh");
                }
            }
            else SetError("Caricamento foto non riuscito. Verifica formato, dimensione e connessione.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Profile] ChangeAvatar ex: {ex.Message}");
        }
    }

    private async Task LoadAchievementBadges()
    {
        try
        {
            if (!IsOwnProfile)
            {
                UnlockedBadges = new(displayedUser?.FeaturedAchievements.Select(id => AchievementsCatalog.All.FirstOrDefault(a => a.Id == id)?.Icon).OfType<string>() ?? []);
                HasUnlockedBadges = UnlockedBadges.Count > 0;
                AchievementSummary = "In evidenza";
                BadgeChoices.Clear();
                return;
            }
            var all = await achievementService.GetAllAsync();
            var unlocked = all.Where(a => a.State.IsUnlocked).ToList();
            var xp = unlocked.Count * 100;
            Tier = $"Livello {xp / 500 + 1} · {xp} XP da achievement";
            BadgeChoices.Clear();
            foreach (var a in unlocked) BadgeChoices.Add(new BadgeChoice { Id = a.Def.Id, Name = a.Def.Name, Icon = a.Def.Icon, IsSelected = displayedUser?.FeaturedAchievements.Contains(a.Def.Id) == true });
            UnlockedBadges = new ObservableCollection<string>(unlocked.Where(a => displayedUser?.FeaturedAchievements.Contains(a.Def.Id) == true).Select(a => a.Def.Icon));
            HasUnlockedBadges = UnlockedBadges.Count > 0;
            AchievementSummary = $"{unlocked.Count}/{AchievementsCatalog.All.Count} sbloccati";
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Profile Badges] ex: {ex.Message}"); }
    }

    [RelayCommand] private Task ShowFollowersAsync() => ShowConnectionsAsync(true);
    [RelayCommand] private Task ShowFollowingAsync() => ShowConnectionsAsync(false);
    [RelayCommand] private void CloseConnections() => IsConnectionsVisible = false;
    private async Task ShowConnectionsAsync(bool followers)
    {
        var userId = displayedUser?.Id;
        if (userId == null) return;
        Connections.Clear();
        ConnectionsError = "";
        ConnectionsTitle = followers ? "Follower" : "Seguiti";
        IsConnectionsVisible = true;
        IsConnectionsLoading = true;
        ConnectionsMessage = "Caricamento…";
        try
        {
            var visible = await pb.GetVisibleConnectionsAsync(userId);
            CanViewConnections = visible.Allowed;
            relationships = visible.Items;
            if (!visible.Allowed) { ConnectionsMessage = "Questo elenco è privato. Solo gli amici possono vedere gli elenchi impostati su Solo amici."; return; }
            ownRelationships = await pb.GetOwnRelationshipsAsync();
            var ids = followers ? relationships.Where(r => r.ToUser == userId).Select(r => r.FromUser) : relationships.Where(r => r.FromUser == userId).Select(r => r.ToUser);
            foreach (var id in ids.Distinct())
            {
                var user = await pb.GetUserByIdAsync(id);
                if (user == null) continue;
                var item = new UserSearchResult { UserId = id, Name = user.Name, Initial = string.IsNullOrWhiteSpace(user.Name) ? "?" : user.Name[..Math.Min(2, user.Name.Length)].ToUpperInvariant(), CanFollow = id != pb.CurrentUser?.Id, FollowLabel = FollowLabelFor(id), IsFollowing = IsFollowingUser(id) };
                if (!string.IsNullOrWhiteSpace(user.Avatar)) item.AvatarSource = await pb.DownloadAvatarAsync(user.CollectionId, id, user.Avatar);
                item.HasAvatar = item.AvatarSource != null;
                if (user.WorkoutVisibility == "public") item.PublicWorkoutId = await pb.GetLatestPublicWorkoutIdAsync(id);
                Connections.Add(item);
            }
            ConnectionsMessage = "Nessun utente nell’elenco.";
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
        { ConnectionsMessage = "Elenco non disponibile. Chiudi e riprova."; ConnectionsError = ConnectionsMessage; }
        finally { IsConnectionsLoading = false; }
    }
    private bool IsFollowingUser(string id) => ownRelationships.Any(r => r.FromUser == pb.CurrentUser?.Id && r.ToUser == id && r.Status == "accepted");
    private string FollowLabelFor(string id) => IsFollowingUser(id) ? "Segui già" : ownRelationships.Any(r => r.FromUser == pb.CurrentUser?.Id && r.ToUser == id && r.Status == "pending") ? "Richiesta inviata" : "Segui";
    [RelayCommand] private Task FollowProfileAsync() => displayedUser == null || IsOwnProfile ? Task.CompletedTask : FollowConnectionAsync(new UserSearchResult { UserId = displayedUser.Id, Name = Username });
    [RelayCommand] private async Task FollowConnectionAsync(UserSearchResult user)
    {
        if (IsFollowBusy || user.UserId == pb.CurrentUser?.Id) return;
        ConnectionsError = "";
        try
        {
            ownRelationships = await pb.GetOwnRelationshipsAsync();
            if (IsFollowingUser(user.UserId))
            { pendingUnfollow = user; UnfollowName = user.Name; IsUnfollowConfirmationVisible = true; return; }
            if (FollowLabelFor(user.UserId) == "Richiesta inviata") return;
            IsFollowBusy = true;
            if (!await pb.SendFollowRequestAsync(user.UserId)) { SetError("Richiesta non inviata. Riprova."); ConnectionsError = ErrorMessage ?? ""; return; }
            await RefreshFollowStateAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
        { SetError("Impossibile aggiornare il follow."); ConnectionsError = ErrorMessage ?? ""; }
        finally { IsFollowBusy = false; }
    }
    [RelayCommand] private void CancelUnfollow() { if (!IsFollowBusy) { IsUnfollowConfirmationVisible = false; pendingUnfollow = null; } }
    [RelayCommand] private async Task ConfirmUnfollowAsync()
    {
        if (pendingUnfollow == null || IsFollowBusy) return;
        IsFollowBusy = true;
        try
        {
            if (!await pb.UnfollowUserAsync(pendingUnfollow.UserId)) { SetError("Follow non rimosso. Riprova."); ConnectionsError = ErrorMessage ?? ""; return; }
            IsUnfollowConfirmationVisible = false;
            pendingUnfollow = null;
            await RefreshFollowStateAsync();
            var visible = await pb.GetVisibleConnectionsAsync(displayedUser!.Id);
            CanViewConnections = visible.Allowed;
            relationships = visible.Items;
            FollowersCount = relationships.Where(r => r.ToUser == displayedUser.Id).Select(r => r.FromUser).Distinct().Count();
            FollowingCount = relationships.Where(r => r.FromUser == displayedUser.Id).Select(r => r.ToUser).Distinct().Count();
            // Revoked reciprocal friendship can also revoke access to private lists and workouts.
            if (!CanViewConnections) { Connections.Clear(); ConnectionsMessage = "Questo elenco è privato."; }
            else if (IsConnectionsVisible && ConnectionsTitle == "Seguiti" && IsOwnProfile)
                foreach (var item in Connections.Where(c => !IsFollowingUser(c.UserId)).ToList()) Connections.Remove(item);
            await LoadRealWorkouts();
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
        { SetError("Impossibile aggiornare il follow."); ConnectionsError = ErrorMessage ?? ""; }
        finally { IsFollowBusy = false; }
    }
    private async Task RefreshFollowStateAsync()
    {
        ownRelationships = await pb.GetOwnRelationshipsAsync();
        ProfileFollowLabel = FollowLabelFor(displayedUser?.Id ?? "");
        foreach (var item in Connections) { item.IsFollowing = IsFollowingUser(item.UserId); item.FollowLabel = FollowLabelFor(item.UserId); }
    }
    [RelayCommand] private async Task OpenConnectionStoryAsync(UserSearchResult user)
    {
        if (!user.HasPublicWorkout) { await OpenUserProfileAsync(user); return; }
        await Shell.Current.GoToAsync($"workoutDetail?workoutId={Uri.EscapeDataString(user.PublicWorkoutId)}&source=pocketbase");
    }
    [RelayCommand] private async Task OpenUserProfileAsync(UserSearchResult user)
    {
        IsConnectionsVisible = false;
        await Shell.Current.GoToAsync($"profile?userId={Uri.EscapeDataString(user.UserId)}");
    }
    [RelayCommand] private void ToggleBadge(BadgeChoice badge)
    {
        badge.IsSelected = !badge.IsSelected;
        ErrorMessage = null;
    }
}

public partial class BadgeChoice : ObservableObject
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    [ObservableProperty] private bool isSelected;
}
