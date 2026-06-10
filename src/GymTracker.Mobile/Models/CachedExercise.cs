using SQLite;

namespace GymTracker.Mobile.Models;

[Table("cached_exercises")]
public class CachedExercise
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string InstructionsJson { get; set; } = "[]";
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Force { get; set; } = string.Empty;
    public string Mechanic { get; set; } = string.Empty;

    [Indexed]
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}
