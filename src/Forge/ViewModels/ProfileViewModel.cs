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

public partial class ProfileViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;
    private readonly AchievementService achievementService;

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

    public ProfileViewModel(PocketBaseService pb, AchievementService achievementService)
    {
        this.pb = pb;
        this.achievementService = achievementService;
        HasData = true;

        WeakReferenceMessenger.Default.Register<WorkoutSavedMessage>(this, async (_, _) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await LoadAsync());
        });
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoggedIn = pb.IsLoggedIn;

        if (pb.IsLoggedIn && pb.CurrentUser != null)
        {
            var user = pb.CurrentUser;
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
                AvatarUrl = pb.GetFileUrl(user.CollectionId, user.Id, user.Avatar);
                AvatarSource = ImageSource.FromUri(new Uri(AvatarUrl));
                HasAvatar = true;
                System.Diagnostics.Debug.WriteLine($"[Profile] AvatarUrl={AvatarUrl}");
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
        }
        else
        {
            Username = "Offline User";
            Tier = "Demo Mode";
            AvatarInitials = "GT";
            HasAvatar = false;
        }
    }

    private async Task LoadRealWorkouts()
    {
        WorkoutLoadError = string.Empty;
        HasWorkoutError = false;
        try
        {
            System.Diagnostics.Debug.WriteLine("[Profile] LoadRealWorkouts starting...");
            var workouts = await pb.GetMyWorkoutsAsync(10);
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
                    Title = w.Name,
                    Date = dateStr,
                    Exercises = string.Join(", ", w.Exercises ?? new()),
                    Duration = $"{w.Duration} min",
                    Volume = $"{w.Volume:0.#} kg",
                    Likes = w.Likes,
                    BorderColor = colors[ci % colors.Length],
                    Notes = w.Notes ?? "",
                    HasNotes = !string.IsNullOrWhiteSpace(w.Notes),
                    FirstPhoto = w.Photos.FirstOrDefault() ?? "",
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
            var workouts = await pb.GetMyWorkoutsAsync(365);
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
        EditName = Username;
        EditBio = Bio;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task OpenWorkoutDetailAsync(ProfileWorkout workout)
    {
        if (string.IsNullOrWhiteSpace(workout?.WorkoutId)) return;
        await Shell.Current.GoToAsync($"workoutDetail?workoutId={Uri.EscapeDataString(workout.WorkoutId)}&source=pocketbase");
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
        var (success, error) = await pb.UpdateUserAsync(name: EditName, bio: EditBio);

        if (success)
        {
            Username = EditName;
            Bio = EditBio;
            IsEditing = false;
            SetSuccess(true);
        }
        else
        {
            SetError(error);
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ChangeAvatarAsync()
    {
        if (!pb.IsLoggedIn) return;

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
                    AvatarUrl = pb.GetFileUrl(user.CollectionId, user.Id, user.Avatar);
                    AvatarSource = ImageSource.FromUri(new Uri(AvatarUrl));
                    HasAvatar = true;
                    System.Diagnostics.Debug.WriteLine($"[Profile] New AvatarUrl={AvatarUrl}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Profile] Avatar still empty after upload/refresh");
                }
            }
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
            var all = await achievementService.GetAllAsync();
            var unlocked = all.Where(a => a.State.IsUnlocked).ToList();
            UnlockedBadges = new ObservableCollection<string>(unlocked.Select(a => a.Def.Icon));
            HasUnlockedBadges = UnlockedBadges.Count > 0;
            AchievementSummary = $"{unlocked.Count}/{AchievementsCatalog.All.Count} sbloccati";
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Profile Badges] ex: {ex.Message}"); }
    }
}
