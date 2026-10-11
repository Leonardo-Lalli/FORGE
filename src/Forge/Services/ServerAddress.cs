namespace Forge.Services;

public static class ServerAddress
{
    public const string PublicUrl = "https://leoforge.duckdns.org";

    public static bool TryNormalize(string? value, out string url)
    {
        url = PublicUrl;
        if (string.IsNullOrWhiteSpace(value)) return true;
        var candidate = value.Trim();
        if (!candidate.Contains("://")) candidate = "https://" + candidate;
        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "https" && uri.Scheme != "http") ||
            string.IsNullOrEmpty(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment)) return false;
        url = uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
        if (url.EndsWith("/api", StringComparison.OrdinalIgnoreCase)) url = url[..^4];
        // The shared instance always uses TLS, even with an old HTTP preference.
        if (uri.Host.Equals("leoforge.duckdns.org", StringComparison.OrdinalIgnoreCase)) url = PublicUrl;
        return true;
    }
}
