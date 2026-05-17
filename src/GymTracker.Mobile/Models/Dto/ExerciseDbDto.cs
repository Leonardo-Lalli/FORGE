using System.Text.Json.Serialization;

namespace GymTracker.Mobile.Models.Dto;

public class ExerciseDbDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("equipment")]
    public string Equipment { get; set; } = string.Empty;

    [JsonPropertyName("force")]
    public string Force { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public string Level { get; set; } = string.Empty;

    [JsonPropertyName("mechanic")]
    public string Mechanic { get; set; } = string.Empty;

    [JsonPropertyName("primaryMuscles")]
    public List<string> PrimaryMuscles { get; set; } = new();

    [JsonPropertyName("secondaryMuscles")]
    public List<string> SecondaryMuscles { get; set; } = new();

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = new();

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();
}

public class ExerciseListResponse
{
    [JsonPropertyName("excercises_ids")]
    public List<string> ExerciseIds { get; set; } = new();
}
