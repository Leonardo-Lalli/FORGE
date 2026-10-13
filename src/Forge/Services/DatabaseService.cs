using SQLite;
using Forge.Models;

namespace Forge.Services;

public class DatabaseService
{
    static DatabaseService() => SQLitePCL.Batteries_V2.Init();
    private SQLiteAsyncConnection? db;
    private readonly string dbPath;
    private readonly SemaphoreSlim initialization = new(1, 1);

    public DatabaseService() : this(Path.Combine(FileSystem.AppDataDirectory, "forge.db"))
    {
    }

    public DatabaseService(string databasePath)
    {
        dbPath = databasePath;
    }

    private async Task<SQLiteAsyncConnection> GetDbAsync()
    {
        if (db != null) return db;

        await initialization.WaitAsync();
        try
        {
            if (db != null) return db;
            var connection = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            // CreateTable migrates added columns; publish only after all tables exist.
            await connection.CreateTableAsync<LocalWorkout>();
            await connection.CreateTableAsync<CachedExercise>();
            await connection.CreateTableAsync<SavedPlan>();
            await connection.CreateTableAsync<AchievementState>();
            db = connection;
            return db;
        }
        finally { initialization.Release(); }
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
        return await d.Table<LocalWorkout>().Where(w => w.PendingSync && !w.IsDraft).ToListAsync();
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

    public async Task MarkSyncedAsync(string workoutId, string? remoteId = null)
    {
        var d = await GetDbAsync();
        var w = await d.FindAsync<LocalWorkout>(workoutId);
        if (w != null)
        {
            w.PendingSync = false;
            w.LastSynced = DateTime.UtcNow;
            if (remoteId != null && remoteId != workoutId)
            {
                w.Id = remoteId;
                await d.RunInTransactionAsync(connection =>
                {
                    connection.InsertOrReplace(w);
                    connection.Delete<LocalWorkout>(workoutId);
                });
            }
            else await d.UpdateAsync(w);
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

    public async Task<CachedExercise?> GetCachedExerciseByIdAsync(string id)
    {
        var d = await GetDbAsync();
        return await d.FindAsync<CachedExercise>(id);
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

    public async Task<List<SavedPlan>> GetPlansAsync(string? userId = null)
    {
        var d = await GetDbAsync();
        return userId == null ? await d.Table<SavedPlan>().OrderByDescending(p => p.UpdatedAt).ToListAsync() : await d.Table<SavedPlan>().Where(p => p.UserId == userId).OrderByDescending(p => p.UpdatedAt).ToListAsync();
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

    public async Task DeleteAllExercisesAsync()
    {
        var d = await GetDbAsync();
        await d.DeleteAllAsync<CachedExercise>();
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
    public async Task DeleteAccountDataAsync(string userId)
    {
        var d = await GetDbAsync();
        await d.RunInTransactionAsync(c => {
            c.Execute("DELETE FROM local_workouts WHERE UserId = ?", userId);
            c.Execute("DELETE FROM saved_plans WHERE UserId = ?", userId);
            c.Execute("DELETE FROM achievements WHERE Id LIKE ?", userId + ":%");
        });
    }
}
