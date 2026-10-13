namespace Forge.Services;

public static class ExerciseMuscleFilter
{
    // Primary muscles take precedence over broad body regions and secondary stabilizers.
    public static bool Matches(string bodyPart, string targets, string category, IEnumerable<string> aliases)
    {
        var primary = Tokens(targets);
        var values = aliases.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (primary.Count > 0) return primary.Any(values.Contains);
        return Tokens(bodyPart).Concat(Tokens(category)).Any(values.Contains);
    }
    private static List<string> Tokens(string value) => value.Split([',', ';', '|', '[', ']', '"'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(Normalize).ToList();
    private static string Normalize(string value) => value.Trim().ToLowerInvariant().Replace('_', ' ').Replace('-', ' ');
}
