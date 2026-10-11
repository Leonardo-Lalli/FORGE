using System.Text.Json.Serialization;

namespace Forge.Models.Dto;

public class PocketBaseAuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("record")]
    public PocketBaseUserRecord Record { get; set; } = new();
}

public class PocketBaseUserRecord
{
    private List<string> featuredAchievements = new();
    [JsonPropertyName("workout_visibility")]
    public string WorkoutVisibility { get; set; } = "";
    [JsonPropertyName("connections_visibility")]
    public string ConnectionsVisibility { get; set; } = "";
    [JsonPropertyName("featured_achievements")]
    public List<string> FeaturedAchievements { get => featuredAchievements; set => featuredAchievements = value ?? new(); }
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("bio")]
    public string Bio { get; set; } = string.Empty;

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;

    [JsonPropertyName("collectionId")]
    public string CollectionId { get; set; } = string.Empty;

    [JsonPropertyName("collectionName")]
    public string CollectionName { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public string Created { get; set; } = string.Empty;

    [JsonPropertyName("updated")]
    public string Updated { get; set; } = string.Empty;
}

public class PocketBaseListResponse<T>
{
    private List<T> items = new();
    [JsonPropertyName("items")]
    public List<T> Items { get => items; set => items = value ?? new(); }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("perPage")]
    public int PerPage { get; set; }

    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}

public class SocialGraphRecord
{
    private string? fromName;
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("from_user")]
    public string FromUser { get; set; } = string.Empty;

    [JsonPropertyName("from_name")]
    public string FromName { get => fromName ?? LegacyFromName ?? string.Empty; set => fromName = value; }

    [JsonPropertyName("form_name")]
    public string? LegacyFromName { get; set; }

    [JsonPropertyName("to_user")]
    public string ToUser { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = "pending";

    [JsonPropertyName("created")]
    public string Created { get; set; } = string.Empty;
}

public class LoggedWorkoutRecord
{
    private List<string> exercises = new(), likedBy = new(), photos = new();
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public string User { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("exercises")]
    public List<string> Exercises { get => exercises; set => exercises = value ?? new(); }

    [JsonPropertyName("volume")]
    public double Volume { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;

    [JsonPropertyName("exercise_data")]
    [JsonConverter(typeof(JsonTextConverter))]
    public string ExerciseData { get; set; } = string.Empty;

    [JsonPropertyName("likes")]
    public int Likes { get; set; }

    [JsonPropertyName("liked_by")]
    public List<string> LikedBy { get => likedBy; set => likedBy = value ?? new(); }

    [JsonPropertyName("user_name")]
    public string UserName { get; set; } = string.Empty;

    [JsonIgnore]
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonPropertyName("photos")]
    public List<string> Photos { get => photos; set => photos = value ?? new(); }
}

public class JsonTextConverter : JsonConverter<string>
{
    public override string Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        if (reader.TokenType == System.Text.Json.JsonTokenType.String) return reader.GetString() ?? "[]";
        using var doc = System.Text.Json.JsonDocument.ParseValue(ref reader);
        return doc.RootElement.GetRawText();
    }
    public override void Write(System.Text.Json.Utf8JsonWriter writer, string value, System.Text.Json.JsonSerializerOptions options) => writer.WriteStringValue(value);
}
