using Forge.Models.Dto;

namespace Forge.Services;

public record VolumeBucket(string Label, double Volume, double Height);
public static class VolumeChart
{
    public static List<VolumeBucket> Build(IEnumerable<LoggedWorkoutRecord> source, string filter, DateTime today)
    {
        var days = source.Select(w => (Date: AchievementEvaluator.ParseDate(w.Date), Volume: w.Volume))
            .Where(w => w.Date.HasValue && double.IsFinite(w.Volume) && w.Volume >= 0)
            .GroupBy(w => w.Date!.Value.Date).ToDictionary(g => g.Key, g => g.Sum(w => w.Volume));
        if (days.Count == 0) return [];
        var start = StatsPeriod.Start(filter, today, days.Keys.Min());
        var step = filter is "week" or "month" ? 1 : 7;
        var buckets = new List<(string Label, double Volume)>();
        for (var cursor = start; cursor <= today; cursor = cursor.AddDays(step))
        {
            var volume = days.Where(d => d.Key >= cursor && d.Key < cursor.AddDays(step)).Sum(d => d.Value);
            buckets.Add((cursor.ToString("dd/MM"), volume));
        }
        var max = buckets.Select(b => b.Volume).DefaultIfEmpty().Max();
        return buckets.Select(b => new VolumeBucket(b.Label, b.Volume, max > 0 ? b.Volume / max * 140 : 0)).ToList();
    }
}
