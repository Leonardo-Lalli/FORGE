using Forge.Models;

namespace Forge.Services;

public class AchievementService
{
    private readonly DatabaseService db;
    private readonly PocketBaseService pb;
    private readonly SemaphoreSlim gate = new(1, 1);

    public AchievementService(DatabaseService db, PocketBaseService pb)
    {
        this.db = db;
        this.pb = pb;
    }

    public async Task<List<(AchievementDefinition Def, AchievementState State)>> GetAllAsync()
    {
        await gate.WaitAsync();
        try
        {
            var userId = pb.CurrentUser?.Id;
            if (string.IsNullOrEmpty(userId))
                return AchievementsCatalog.All.Select(d => (d, new AchievementState { Id = d.Id, MaxProgress = d.MaxProgress })).ToList();

            var progress = AchievementEvaluator.Evaluate(await db.GetWorkoutsAsync(userId));
            var saved = (await db.GetAchievementsAsync()).ToDictionary(s => s.Id);
            var result = new List<(AchievementDefinition Def, AchievementState State)>();
            foreach (var def in AchievementsCatalog.All)
            {
                // Legacy unscoped states cannot be attributed to an account safely.
                var key = $"{userId}:{def.Id}";
                var state = saved.GetValueOrDefault(key) ?? new AchievementState { Id = key };
                state.MaxProgress = def.MaxProgress;
                state.Progress = Math.Clamp(Math.Max(state.Progress, progress.GetValueOrDefault(def.Id)), 0, def.MaxProgress);
                if (state.Progress >= def.MaxProgress && !state.IsUnlocked)
                {
                    state.IsUnlocked = true;
                    state.UnlockedAt = DateTime.UtcNow.ToString("o");
                }
                if (def.Id != "legend") await db.SaveAchievementAsync(state);
                result.Add((def, state));
            }
            var legend = result.Single(a => a.Def.Id == "legend").State;
            if (result.Where(a => a.Def.Id != "legend").All(a => a.State.IsUnlocked))
            {
                legend.Progress = 1;
                legend.IsUnlocked = true;
                if (string.IsNullOrEmpty(legend.UnlockedAt)) legend.UnlockedAt = DateTime.UtcNow.ToString("o");
                await db.SaveAchievementAsync(legend);
            }
            return result;
        }
        finally { gate.Release(); }
    }

    public async Task<int> GetUnlockedCountAsync() => (await GetAllAsync()).Count(a => a.State.IsUnlocked);

    // MAUI ProgressBar takes a ratio from zero to one.
    public async Task<double> GetProgressPercentAsync() =>
        AchievementsCatalog.All.Count == 0 ? 0 : (double)await GetUnlockedCountAsync() / AchievementsCatalog.All.Count;

    public async Task<List<(AchievementDefinition Def, AchievementState State)>> GetRecentUnlocksAsync(int count = 3) =>
        (await GetAllAsync()).Where(a => a.State.IsUnlocked).OrderByDescending(a => a.State.UnlockedAt).Take(count).ToList();

    public async Task<TrainingProgress> GetTrainingProgressAsync() =>
        TrainingProgress.FromWorkouts(string.IsNullOrEmpty(pb.CurrentUser?.Id) ? [] : await db.GetWorkoutsAsync(pb.CurrentUser.Id));

    public Task CheckLikeAchievementsAsync() => Task.CompletedTask;

    public async Task CheckFollowAchievementsAsync(int followingCount)
    {
        var userId = pb.CurrentUser?.Id;
        if (string.IsNullOrEmpty(userId)) return;
        await gate.WaitAsync();
        try
        {
            var key = $"{userId}:networker";
            var state = (await db.GetAchievementsAsync()).FirstOrDefault(s => s.Id == key)
                ?? new AchievementState { Id = key, MaxProgress = 5 };
            state.Progress = Math.Clamp(Math.Max(state.Progress, followingCount), 0, 5);
            if (state.Progress == 5 && !state.IsUnlocked)
            {
                state.IsUnlocked = true;
                state.UnlockedAt = DateTime.UtcNow.ToString("o");
            }
            await db.SaveAchievementAsync(state);
        }
        finally { gate.Release(); }
    }
}
