using SQLite;
using GymTracker.Mobile.Models;

namespace GymTracker.Mobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? db;
    private readonly string dbPath;

    public DatabaseService()
    {
        dbPath = Path.Combine(FileSystem.AppDataDirectory, "forge.db");
    }

    private async Task<SQLiteAsyncConnection> GetDbAsync()
    {
        if (db != null) return db;

        db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await db.CreateTableAsync<LocalWorkout>();
        await db.CreateTableAsync<CachedExercise>();
        await db.CreateTableAsync<SavedPlan>();
        await db.CreateTableAsync<AchievementState>();

        try { await db.ExecuteAsync("ALTER TABLE local_workouts ADD COLUMN IsDraft INTEGER DEFAULT 0"); }
        catch { /* column already exists */ }

        return db;
    }

    public async Task<List<LocalWorkout>> GetWorkoutsAsync(string? userId = null)
    {
        var d = await GetDbAsync();
        if (userId != null)
            return await d.Table<LocalWorkout>().Where(w => w.UserId == userId).OrderByDescending(w => w.Date).ToListAsync();
        return await d.Table<LocalWorkout>().OrderByDescending(w => w.Date).ToListAsync();
    }

    public async Task<List<LocalWorkout>> GetPendingSyncAsync()
    {
        var d = await GetDbAsync();
        return await d.Table<LocalWorkout>().Where(w => w.PendingSync).ToListAsync();
    }

    public async Task SaveWorkoutAsync(LocalWorkout workout)
    {
        var d = await GetDbAsync();
        var existing = await d.FindAsync<LocalWorkout>(workout.Id);
        if (existing != null)
            await d.UpdateAsync(workout);
        else
            await d.InsertAsync(workout);
    }

    public async Task MarkSyncedAsync(string workoutId)
    {
        var d = await GetDbAsync();
        var w = await d.FindAsync<LocalWorkout>(workoutId);
        if (w != null)
        {
            w.PendingSync = false;
            w.LastSynced = DateTime.UtcNow;
            await d.UpdateAsync(w);
        }
    }

    public async Task<int> DeleteWorkoutAsync(string workoutId)
    {
        var d = await GetDbAsync();
        return await d.DeleteAsync<LocalWorkout>(workoutId);
    }

    public async Task<List<CachedExercise>> GetCachedExercisesAsync(string? bodyPart = null)
    {
        var d = await GetDbAsync();
        if (bodyPart != null)
            return await d.Table<CachedExercise>().Where(e => e.BodyPart == bodyPart).ToListAsync();
        return await d.Table<CachedExercise>().ToListAsync();
    }

    public async Task<CachedExercise?> GetCachedExerciseByNameAsync(string name)
    {
        var d = await GetDbAsync();
        return await d.Table<CachedExercise>().Where(e => e.Name == name).FirstOrDefaultAsync();
    }

    public async Task SaveCachedExerciseAsync(CachedExercise exercise)
    {
        var d = await GetDbAsync();
        var existing = await d.FindAsync<CachedExercise>(exercise.Id);
        if (existing != null)
            await d.UpdateAsync(exercise);
        else
            await d.InsertAsync(exercise);
    }

    public async Task<List<SavedPlan>> GetPlansAsync()
    {
        var d = await GetDbAsync();
        return await d.Table<SavedPlan>().OrderByDescending(p => p.UpdatedAt).ToListAsync();
    }

    public async Task SavePlanAsync(SavedPlan plan)
    {
        var d = await GetDbAsync();
        var existing = await d.FindAsync<SavedPlan>(plan.Id);
        if (existing != null)
            await d.UpdateAsync(plan);
        else
            await d.InsertAsync(plan);
    }

    public async Task DeleteExercisesByPrefixAsync(string prefix)
    {
        var d = await GetDbAsync();
        await d.ExecuteAsync("DELETE FROM cached_exercises WHERE Id LIKE ?", $"{prefix}%");
    }

    public async Task DeletePlanAsync(string planId)
    {
        var d = await GetDbAsync();
        await d.DeleteAsync<SavedPlan>(planId);
    }

    public async Task<List<AchievementState>> GetAchievementsAsync()
    {
        var d = await GetDbAsync();
        return await d.Table<AchievementState>().ToListAsync();
    }

    public async Task SaveAchievementAsync(AchievementState state)
    {
        var d = await GetDbAsync();
        var existing = await d.FindAsync<AchievementState>(state.Id);
        if (existing != null)
            await d.UpdateAsync(state);
        else
            await d.InsertAsync(state);
    }
}
