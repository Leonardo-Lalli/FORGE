using SQLite;

namespace Forge.Models;

[Table("saved_plans")]
public class SavedPlan
{
    [Indexed]
    public string UserId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string ExercisesJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int RestSeconds { get; set; } = 90;
}
