using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

public class PlanService
{
    private readonly DatabaseService db;
    private readonly PocketBaseService pb;
    private readonly SemaphoreSlim migrationGate = new(1, 1);
    private readonly SemaphoreSlim starterGate = new(1, 1);
    private readonly SemaphoreSlim planWriteGate = new(1, 1);

    public async Task EnsureStarterPlansAsync()
    {
        var userId = pb.CurrentUser?.Id;
        if (string.IsNullOrEmpty(userId)) return;
        await starterGate.WaitAsync();
        try
        {
            var key = $"starter_plans_v1:{userId}";
            if (Preferences.Get(key, "") == "done") return;
            var existing = await LoadPlansAsync();
            if (existing.Count == 0)
            {
                foreach (var (id, name, names) in new[] {
                    ("full", "Esempio · Full body", new[] { "Bodyweight Squat", "Push Up", "Plank" }),
                    ("light", "Esempio · Movimento leggero", new[] { "Walking", "Bodyweight Lunge", "Stretching" }) })
                {
                    await SavePlanAsync(new WorkoutPlan {
                        Id = $"{userId}:starter:{id}", Name = name,
                        Notes = "Scheda dimostrativa: personalizza esercizi, serie e carichi prima di usarla.",
                        Exercises = names.Select((n, i) => new WorkoutExercise {
                            ExerciseName = n, Order = i + 1, RestSeconds = 60,
                            Sets = new(System.Linq.Enumerable.Range(1, 3).Select(x => new ExerciseSet { SetNumber = x, Reps = 10 }))
                        }).ToList()
                    });
                }
            }
            Preferences.Set(key, "done");
        }
        finally { starterGate.Release(); }
    }
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };
    private string migratedUser = "";

    public PlanService(DatabaseService db, PocketBaseService pb)
    {
        this.db = db;
        this.pb = pb;
    }

    private async Task MigrateFromPreferencesAsync()
    {
        var userId = pb.CurrentUser?.Id;
        if (string.IsNullOrEmpty(userId)) return;
        await migrationGate.WaitAsync();
        try
        {
            if (migratedUser == userId) return;
            // Attribute old local plans once, rather than showing them to every account.
            foreach (var saved in (await db.GetPlansAsync()).Where(p => string.IsNullOrEmpty(p.UserId)))
            {
                saved.UserId = userId;
                await db.SavePlanAsync(saved);
            }
            var json = Preferences.Get("saved_plans", "[]");
            var oldPlans = JsonSerializer.Deserialize<List<WorkoutPlan>>(json, JsonOptions);
            foreach (var plan in oldPlans ?? [])
            {
                await SavePlanAsync(plan);
            }

            Preferences.Remove("saved_plans");
            migratedUser = userId;
        }
        finally { migrationGate.Release(); }
    }

    public async Task<List<WorkoutPlan>> LoadPlansAsync()
    {
        await MigrateFromPreferencesAsync();

        var userId = pb.CurrentUser?.Id;
        if (string.IsNullOrEmpty(userId)) return [];
        var saved = await db.GetPlansAsync(userId);
        return saved.Select(ToWorkoutPlan).ToList();
    }

    public async Task SavePlanAsync(WorkoutPlan plan)
    {
        await planWriteGate.WaitAsync();
        try { await SavePlanCoreAsync(plan); }
        finally { planWriteGate.Release(); }
    }

    public async Task SaveNotesAsync(string planId, string ownerId, string notes,
        IReadOnlyList<PlanSessionChanges.ExerciseNote> exerciseNotes)
    {
        await planWriteGate.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(ownerId) || pb.CurrentUser?.Id != ownerId)
                throw new InvalidOperationException("L’account della sessione è cambiato.");
            var saved = (await db.GetPlansAsync(ownerId)).FirstOrDefault(p => p.Id == planId)
                ?? throw new InvalidOperationException("Scheda non disponibile.");
            var plan = ToWorkoutPlan(saved);
            if (PlanSessionChanges.ApplyNotes(plan, notes, exerciseNotes)) await SavePlanCoreAsync(plan);
        }
        finally { planWriteGate.Release(); }
    }

    private async Task SavePlanCoreAsync(WorkoutPlan plan)
    {
        var userId = pb.CurrentUser?.Id ?? throw new InvalidOperationException("Accedi per salvare una scheda.");
        if (!string.IsNullOrEmpty(plan.UserId) && plan.UserId != userId)
            throw new InvalidOperationException("La scheda appartiene a un altro account.");
        var existing = (await db.GetPlansAsync()).FirstOrDefault(p => p.Id == plan.Id);
        if (existing != null && !string.IsNullOrEmpty(existing.UserId) && existing.UserId != userId)
            throw new InvalidOperationException("La scheda appartiene a un altro account.");
        plan.UserId = userId;
        plan.UpdatedAt = DateTime.Now;
        var sp = new SavedPlan
        {
            Id = plan.Id,
            UserId = userId,
            Notes = plan.Notes,
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
        await planWriteGate.WaitAsync();
        try
        {
            var userId = pb.CurrentUser?.Id;
            if (string.IsNullOrEmpty(userId)) return;
            if (!(await db.GetPlansAsync(userId)).Any(p => p.Id == planId)) return;
            await db.DeletePlanAsync(planId);
        }
        finally { planWriteGate.Release(); }
    }

    private static WorkoutPlan ToWorkoutPlan(SavedPlan sp)
    {
        var exercises = JsonSerializer.Deserialize<List<WorkoutExercise>>(sp.ExercisesJson, JsonOptions) ?? new();
        return new WorkoutPlan
        {
            Id = sp.Id,
            UserId = sp.UserId,
            Notes = sp.Notes,
            Name = sp.Name,
            Exercises = exercises,
            CreatedAt = sp.CreatedAt,
            UpdatedAt = sp.UpdatedAt,
            RestSeconds = sp.RestSeconds
        };
    }
}
