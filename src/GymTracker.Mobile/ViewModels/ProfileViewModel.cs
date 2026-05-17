using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymTracker.Mobile.Services;

namespace GymTracker.Mobile.ViewModels;

public partial class ProfileWorkout : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Exercises { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;
    public string BorderColor { get; set; } = "#ffb4ab";
}

public partial class ProfileViewModel : BaseViewModel
{
    private readonly PocketBaseService pb;

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string tier = string.Empty;
    [ObservableProperty] private string bio = string.Empty;
    [ObservableProperty] private string avatarInitials = "??";
    [ObservableProperty] private string avatarUrl = string.Empty;
    [ObservableProperty] private bool hasAvatar;
    [ObservableProperty] private bool isLoggedIn;
    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private string editName = string.Empty;
    [ObservableProperty] private string editBio = string.Empty;
    [ObservableProperty] private string totalWorkouts = "0";
    [ObservableProperty] private string streakDays = "0";
    [ObservableProperty] private string streakLabel = "Day Streak";
    [ObservableProperty] private string totalVolume = "0";
    [ObservableProperty] private ObservableCollection<ProfileWorkout> recentWorkouts = new();

    public ProfileViewModel(PocketBaseService pb)
    {
        this.pb = pb;
        HasData = true;
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
                HasAvatar = true;
            }
            else
            {
                HasAvatar = false;
            }

            Tier = "FORGE ATHLETE";

            await LoadRealWorkouts();
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
        try
        {
            var workouts = await pb.GetMyWorkoutsAsync(10);
            TotalWorkouts = workouts.Count.ToString();

            double totalVol = 0;
            RecentWorkouts.Clear();

            var colors = new[] { "#ffb4ab", "#00E5FF", "#CCFF00", "#a5d6ff" };
            int ci = 0;

            foreach (var w in workouts)
            {
                totalVol += w.Volume;

                var dateStr = "";
                if (DateTime.TryParse(w.Date, out var dt))
                    dateStr = dt.ToLocalTime().ToString("ddd dd MMM, HH:mm");

                RecentWorkouts.Add(new ProfileWorkout
                {
                    Title = w.Name,
                    Date = dateStr,
                    Exercises = string.Join(", ", w.Exercises ?? new()),
                    Duration = $"{w.Duration} min",
                    Volume = $"{w.Volume:0.#} kg",
                    BorderColor = colors[ci % colors.Length]
                });
                ci++;
            }

            TotalVolume = $"{totalVol:0.#} kg";

            await CalculateStreakAsync();
        }
        catch
        {
            TotalWorkouts = "0";
            TotalVolume = "0 kg";
        }
    }

    private async Task CalculateStreakAsync()
    {
        try
        {
            var workouts = await pb.GetMyWorkoutsAsync(365);
            if (workouts.Count == 0) { StreakDays = "0"; StreakLabel = "Day Streak"; return; }

            var dates = workouts
                .Select(w => { DateTime.TryParse(w.Date, out var d); return d; })
                .Where(d => d != default)
                .Select(d => d.ToLocalTime().Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            int streak = 0;
            var today = DateTime.Now.Date;
            var check = today;

            foreach (var d in dates)
            {
                if (d == check)
                {
                    streak++;
                    check = check.AddDays(-1);
                }
                else if (d < check)
                    break;
            }

            StreakDays = streak.ToString();
            StreakLabel = streak == 1 ? "Day Streak" : "Day Streak";
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
            if (ok)
            {
                await pb.RefreshUserAsync();
                HasAvatar = !string.IsNullOrWhiteSpace(pb.CurrentUser?.Avatar);
                if (HasAvatar)
                    AvatarUrl = pb.GetFileUrl(pb.CurrentUser!.CollectionId, pb.CurrentUser.Id, pb.CurrentUser.Avatar);
            }
        }
        catch { }
    }
}
