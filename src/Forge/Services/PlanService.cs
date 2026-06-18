using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

public class PlanService
{
    private readonly DatabaseService db;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };
    private bool migrated;

    public PlanService(DatabaseService db)
    {
        this.db = db;
    }

    private async Task MigrateFromPreferencesAsync()
    {
        if (migrated) return;
        migrated = true;

        try
        {
            var json = Preferences.Get("saved_plans", "[]");
            if (json == "[]") return;

            var oldPlans = JsonSerializer.Deserialize<List<WorkoutPlan>>(json, JsonOptions);
            if (oldPlans == null || oldPlans.Count == 0) return;

            foreach (var plan in oldPlans)
            {
                await SavePlanAsync(plan);
            }

            Preferences.Remove("saved_plans");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlanService Migration] ex: {ex.Message}");
        }
    }

    public async Task<List<WorkoutPlan>> LoadPlansAsync()
    {
        await MigrateFromPreferencesAsync();

        var saved = await db.GetPlansAsync();
        return saved.Select(ToWorkoutPlan).ToList();
    }

    public async Task SavePlanAsync(WorkoutPlan plan)
    {
        plan.UpdatedAt = DateTime.Now;
        var sp = new SavedPlan
        {
            Id = plan.Id,
            Name = plan.Name,
            ExercisesJson = JsonSerializer.Serialize(plan.Exercises, JsonOptions),
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            RestSeconds = plan.RestSeconds
        };
        await db.SavePlanAsync(sp);
    }

    public async Task DeletePlanAsync(string planId)
    {
        await db.DeletePlanAsync(planId);
    }

    private static WorkoutPlan ToWorkoutPlan(SavedPlan sp)
    {
        var exercises = JsonSerializer.Deserialize<List<WorkoutExercise>>(sp.ExercisesJson, JsonOptions) ?? new();
        return new WorkoutPlan
        {
            Id = sp.Id,
            Name = sp.Name,
            Exercises = exercises,
            CreatedAt = sp.CreatedAt,
            UpdatedAt = sp.UpdatedAt,
            RestSeconds = sp.RestSeconds
        };
    }
}
