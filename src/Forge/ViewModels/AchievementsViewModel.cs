using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Models;
using Forge.Services;

namespace Forge.ViewModels;

public partial class AchievementDisplay : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏆";
    public int Progress { get; set; }
    public int MaxProgress { get; set; } = 1;
    public bool IsUnlocked { get; set; }
    public double ProgressPercent => MaxProgress > 0 ? (double)Progress / MaxProgress : 0;
    public string ProgressText => $"{Progress}/{MaxProgress}";
    public string CategoryIcon => Category switch
    {
        "Costanza" => "📅",
        "Forza" => "💪",
        "Orari" => "⏰",
        "Varietà" => "🧬",
        "Social" => "🦋",
        "Elite" => "👑",
        _ => "🏆"
    };
}

public partial class AchievementsViewModel : BaseViewModel
{
    private readonly AchievementService achievementService;

    [ObservableProperty] private ObservableCollection<AchievementDisplay> achievements = new();
    [ObservableProperty] private ObservableCollection<AchievementDisplay> recentUnlocks = new();
    [ObservableProperty] private string unlockedCount = "0";
    [ObservableProperty] private string totalCount = "0";
    [ObservableProperty] private double progressPercent;
    [ObservableProperty] private string selectedCategory = "Tutti";
    [ObservableProperty] private bool hasRecentUnlocks;

    public AchievementsViewModel(AchievementService achievementService)
    {
        this.achievementService = achievementService;
        HasData = true;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        SetLoading();
        try
        {
            var all = await achievementService.GetAllAsync();
            TotalCount = AchievementsCatalog.All.Count.ToString();
            UnlockedCount = all.Count(a => a.State.IsUnlocked).ToString();
            ProgressPercent = await achievementService.GetProgressPercentAsync();

            var displayList = all.Select(a => new AchievementDisplay
            {
                Id = a.Def.Id,
                Name = a.Def.Name,
                Description = a.Def.Description,
                Category = a.Def.Category,
                Icon = a.Def.Icon,
                Progress = a.State.Progress,
                MaxProgress = a.Def.MaxProgress,
                IsUnlocked = a.State.IsUnlocked
            }).ToList();

            if (SelectedCategory != "Tutti")
                displayList = displayList.Where(a => a.Category == SelectedCategory).ToList();

            Achievements = new ObservableCollection<AchievementDisplay>(displayList);

            var recent = await achievementService.GetRecentUnlocksAsync(5);
            RecentUnlocks = new ObservableCollection<AchievementDisplay>(
                recent.Select(a => new AchievementDisplay
                {
                    Id = a.Def.Id,
                    Name = a.Def.Name,
                    Description = a.Def.Description,
                    Category = a.Def.Category,
                    Icon = a.Def.Icon,
                    Progress = a.State.Progress,
                    MaxProgress = a.Def.MaxProgress,
                    IsUnlocked = true
                }));
            HasRecentUnlocks = RecentUnlocks.Count > 0;

            SetSuccess(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Achievements] Load ex: {ex.Message}");
            SetError("Errore caricamento achievements.");
        }
    }

    [RelayCommand]
    private void FilterCategory(string category)
    {
        SelectedCategory = category;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
