namespace Forge.Services;

public static class CommunityInvite
{
    public const string DownloadUrl = "https://leonardo-lalli.github.io/FORGE/#download";

    public static string Message(string? server)
    {
        if (!ServerAddress.TryNormalize(server, out var url))
            throw new ArgumentException("Indirizzo server non valido.", nameof(server));
        var destination = url == ServerAddress.PublicUrl
            ? "Crea un account sul server FORGE predefinito: ci troviamo nella stessa community."
            : $"Dopo l’installazione, imposta questo indirizzo nelle impostazioni del server e crea un account: {url}";
        return $"Allenati con me su FORGE! Scarica l’app Android dal sito ufficiale.\n\n{destination}";
    }
}
