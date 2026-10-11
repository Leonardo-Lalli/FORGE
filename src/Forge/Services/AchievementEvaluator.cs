using System.Globalization;
using System.Text.Json;
using Forge.Models;

namespace Forge.Services;

/// <summary>Deterministic progress from completed workouts; never uses the time of a UI refresh.</summary>
public static class AchievementEvaluator
{
    public static IReadOnlyDictionary<string, int> Evaluate(IEnumerable<LocalWorkout> source)
    {
        var workouts = source.Where(w => !w.IsDraft).DistinctBy(w => w.Id)
            .Select(w => (Workout: w, Date: ParseDate(w.Date)))
            .Where(w => w.Date.HasValue).OrderBy(w => w.Date).ToList();
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        if (workouts.Count == 0) return result;
        var days = workouts.Select(w => w.Date!.Value.Date).Distinct().Order().ToList();
        var longest = 1;
        var streak = 1;
        for (var i = 1; i < days.Count; i++)
        {
            streak = days[i] == days[i - 1].AddDays(1) ? streak + 1 : 1;
            longest = Math.Max(longest, streak);
        }
        result["first_workout"] = 1;
        result["member"] = result["veteran"] = workouts.Count;
        result["holy_week"] = result["unstoppable"] = longest;
        result["habit"] = workouts.GroupBy(w => (w.Date!.Value.Year, w.Date.Value.Month)).Max(g => g.Count());
        result["weekend_warrior"] = days.Where(d => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            .GroupBy(WeekStart).Select(g => g.Count()).DefaultIfEmpty().Max();
        result["comeback"] = days.Zip(days.Skip(1)).Any(p => (p.Second - p.First).TotalDays >= 14) ? 1 : 0;
        result["early_bird"] = workouts.Any(w => w.Date!.Value.Hour < 7) ? 1 : 0;
        result["night_owl"] = workouts.Any(w => w.Date!.Value.Hour >= 22) ? 1 : 0;
        result["max_weight"] = Whole(workouts.Max(w => w.Workout.Volume));
        result["iron_marathon"] = Whole(workouts.GroupBy(w => WeekStart(w.Date!.Value.Date)).Max(g => g.Sum(w => Valid(w.Workout.Volume))));
        result["ton_week"] = Whole(workouts.GroupBy(w => (w.Date!.Value.Year, w.Date.Value.Month)).Max(g => g.Sum(w => Valid(w.Workout.Volume))));
        result["hour_grinder"] = result["milestone"] = Whole(workouts.Sum(w => Math.Max(0L, w.Workout.Duration)) / 60.0);
        result["photographer"] = workouts.Count(w => ReadStrings(w.Workout.PhotosJson).Count > 0);
        result["narcissus"] = result["photographer"] > 0 ? 1 : 0;
        result["explorer"] = workouts.Max(w => ReadStrings(w.Workout.ExercisesJson).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        result["encyclopedia"] = workouts.SelectMany(w => ReadStrings(w.Workout.ExercisesJson)).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        return result;
    }

    public static DateTime? ParseDate(string value) =>
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date)
            ? date.LocalDateTime : null;

    private static DateTime WeekStart(DateTime day) => day.AddDays(-((int)day.DayOfWeek + 6) % 7);
    private static double Valid(double value) => double.IsFinite(value) ? Math.Max(0, value) : 0;
    private static int Whole(double value) => (int)Math.Min(int.MaxValue, Valid(value));
    private static List<string> ReadStrings(string json)
    {
        try { return (JsonSerializer.Deserialize<List<string>>(json) ?? []).Where(s => !string.IsNullOrWhiteSpace(s)).ToList(); }
        catch (JsonException) { return []; }
    }
}
