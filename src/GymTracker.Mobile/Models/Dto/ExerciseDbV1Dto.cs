using System.Text.Json;
using System.Text.Json.Serialization;

namespace GymTracker.Mobile.Models.Dto;

public class ExerciseDbV1Dto
{
    [JsonPropertyName("exerciseId")]
    public string ExerciseId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("gifUrl")]
    public string GifUrl { get; set; } = string.Empty;

    [JsonPropertyName("bodyParts")]
    public object? BodyPartsRaw { get; set; }

    [JsonPropertyName("equipments")]
    public object? EquipmentsRaw { get; set; }

    [JsonPropertyName("targetMuscles")]
    public object? TargetMusclesRaw { get; set; }

    [JsonPropertyName("secondaryMuscles")]
    public object? SecondaryMusclesRaw { get; set; }

    [JsonPropertyName("instructions")]
    public object? InstructionsRaw { get; set; }

    public List<string> GetBodyParts() => NormalizeList(BodyPartsRaw);
    public List<string> GetEquipments() => NormalizeList(EquipmentsRaw);
    public List<string> GetTargetMuscles() => NormalizeList(TargetMusclesRaw);
    public List<string> GetSecondaryMuscles() => NormalizeList(SecondaryMusclesRaw, " ");
    public List<string> GetInstructions() => NormalizeList(InstructionsRaw);

    private static List<string> NormalizeList(object? raw, string? splitSeparator = null)
    {
        if (raw == null) return new();
        if (raw is JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Array)
                return el.EnumerateArray().Select(e => e.GetString() ?? "").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (el.ValueKind == JsonValueKind.String)
            {
                var str = el.GetString() ?? "";
                return splitSeparator != null
                    ? str.Split(splitSeparator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string> { str };
            }
        }
        return new();
    }
}

public class ExerciseDbV1ListResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<ExerciseDbV1Dto> Data { get; set; } = new();

    [JsonPropertyName("meta")]
    public ExerciseDbV1Meta? Meta { get; set; }
}

public class ExerciseDbV1DetailResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public ExerciseDbV1Dto? Data { get; set; }
}

public class ExerciseDbV1Meta
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }

    [JsonPropertyName("nextCursor")]
    public string NextCursor { get; set; } = string.Empty;
}
