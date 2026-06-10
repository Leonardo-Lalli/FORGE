using SQLite;

namespace GymTracker.Mobile.Models;

[Table("local_workouts")]
public class LocalWorkout
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string ExercisesJson { get; set; } = "[]";
    public double Volume { get; set; }
    public int Duration { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string ExerciseDataJson { get; set; } = string.Empty;
    public int Likes { get; set; }
    public string LikedByJson { get; set; } = "[]";
    public string UserName { get; set; } = string.Empty;

    [Indexed]
    public bool PendingSync { get; set; }
    public DateTime LastSynced { get; set; } = DateTime.MinValue;
}
