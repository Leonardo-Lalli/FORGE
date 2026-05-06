using System.Text.Json;
using GymTracker.Mobile.Models;

namespace GymTracker.Mobile.Services;

public static class PlanStore
{
    private const string Key = "saved_plans";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    public static List<WorkoutPlan> LoadPlans()
    {
        var json = Preferences.Get(Key, "[]");
        return JsonSerializer.Deserialize<List<WorkoutPlan>>(json, JsonOptions) ?? new();
    }

    public static void SavePlan(WorkoutPlan plan)
    {
        var plans = LoadPlans();
        plans.Insert(0, plan);
        var json = JsonSerializer.Serialize(plans, JsonOptions);
        Preferences.Set(Key, json);
    }

    public static void DeletePlan(string planId)
    {
        var plans = LoadPlans();
        plans.RemoveAll(p => p.Id == planId);
        var json = JsonSerializer.Serialize(plans, JsonOptions);
        Preferences.Set(Key, json);
    }
}
