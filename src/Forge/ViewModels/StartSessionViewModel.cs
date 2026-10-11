using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forge.Models;
using Forge.Services;

namespace Forge.ViewModels;

public partial class ProtocolCard : ObservableObject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ExerciseCount { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string Intensity { get; set; } = string.Empty;
    public string IntensityColor { get; set; } = "Primary";
}

public partial class StartSessionViewModel : BaseViewModel
{
    private readonly PlanService planService;

    [ObservableProperty] private ObservableCollection<ProtocolCard> protocols = new();

    public StartSessionViewModel(PlanService planService)
    {
        this.planService = planService;
        HasData = true;
        _ = InitializeAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                System.Diagnostics.Debug.WriteLine($"[StartSession Init] ex: {t.Exception?.InnerException?.Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private async Task InitializeAsync()
    {
        await planService.EnsureStarterPlansAsync();
        await LoadProtocolsAsync();
    }

    public async Task LoadProtocolsAsync()
    {
        var plans = await planService.LoadPlansAsync();
        Protocols.Clear();
        foreach (var plan in plans)
        {
            Protocols.Add(new ProtocolCard
            {
                Id = plan.Id,
                Name = plan.Name,
                ExerciseCount = plan.Exercises.Count,
                Duration = $"{plan.Exercises.Count * 5 + plan.RestSeconds / 60 * plan.Exercises.Sum(e => e.Sets.Count)}m",
                Intensity = plan.Exercises.Count switch
                {
                    >= 6 => "High",
                    >= 4 => "Medium",
                    _ => "Low"
                },
                IntensityColor = plan.Exercises.Count switch
                {
                    >= 6 => "LimeGreen",
                    >= 4 => "Primary",
                    _ => "TextSecondary"
                }
            });
        }
    }

    [RelayCommand]
    private async Task QuickStartAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "free"
        });
    }

    [RelayCommand]
    private async Task CreateNewPlanAsync()
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "create"
        });
    }

    [RelayCommand]
    private async Task StartProtocolAsync(ProtocolCard protocol)
    {
        await Shell.Current.GoToAsync("activeWorkout", new Dictionary<string, object>
        {
            ["mode"] = "plan",
            ["planId"] = protocol.Id
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteProtocolAsync(ProtocolCard protocol)
    {
        await planService.DeletePlanAsync(protocol.Id);
        await LoadProtocolsAsync();
    }
}
