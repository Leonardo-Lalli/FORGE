# API Notes — FORGE

## PocketBase API

### Base URL
```
https://{server}/api
```

Tutte le richieste autenticate richiedono header:
```
Authorization: Bearer {token}
```

### Auth

| Endpoint | Method | Body | Response |
|----------|--------|------|----------|
| `/collections/users/auth-with-password` | POST | `{ identity, password }` | `{ token, record }` |
| `/collections/users/records` | POST | `{ email, password, passwordConfirm, name }` | `{ id, email, name }` |
| `/collections/users/records/{id}` | GET | — | `{ id, email, name, bio, avatar, collectionId }` |
| `/collections/users/records/{id}` | PATCH | `{ name, bio }` | Record aggiornato |
| `/collections/users/records/{id}` | PATCH (multipart) | `avatar` file field | Record con avatar |
| `/api/files/{collectionId}/{recordId}/{fileName}` | GET | `?token=` | File binario |

### CRUD

Tutte le richieste CRUD seguono lo schema PocketBase standard:

```
GET    /collections/{collection}/records?perPage={n}
GET    /collections/{collection}/records?search={query}
GET    /collections/{collection}/records/{id}
POST   /collections/{collection}/records          → { ...campi }
PATCH  /collections/{collection}/records/{id}     → { ...campi }
DELETE /collections/{collection}/records/{id}
```

### Parsing robusto (JsonDocument)

Alcuni campi PocketBase (es. `user` nelle relazioni) possono essere restituiti come stringa `"abc123"` o oggetto `{"id":"abc123"}`. Il parsing usa `JsonDocument` per gestire entrambi:

```csharp
if (item.TryGetProperty("user", out var userEl))
{
    if (userEl.ValueKind == JsonValueKind.String)
        record.User = userEl.GetString() ?? "";
    else if (userEl.ValueKind == JsonValueKind.Object && userEl.TryGetProperty("id", out var uidEl))
        record.User = uidEl.GetString() ?? "";
}
```

Questo evita `JsonException` che si verificherebbe con `ReadFromJsonAsync<T>` quando il formato cambia.

## ExerciseDB API (RapidAPI)

### Base URL
```
https://exercise-db-fitness-workout-gym.p.rapidapi.com
```

### Headers
```
X-RapidAPI-Key: {api_key}
X-RapidAPI-Host: exercise-db-fitness-workout-gym.p.rapidapi.com
```

### Endpoint usati

| Endpoint | Query | Uso |
|----------|-------|-----|
| `/exercises` | `?limit=1300&offset=0` | Carica tutti gli ID esercizi |
| `/exercise/{id}` | — | Dettaglio esercizio (immagini, istruzioni, muscoli) |
| `/exercises/name/{name}` | `?limit=20` | Ricerca per nome |

### URL immagini

Le immagini sono short URL (`encr.pw`, `acesse.dev`, `l1nq.com`) che redirectano a URL diretti. Alcuni ISP (Telecom Italia) bloccano questi domini risolvendoli a `127.0.0.1`.

**Soluzione**: `ResolveImageUrlAsync()` usa un `HttpClient` separato con `AllowAutoRedirect = true` e User-Agent Mozilla per seguire i redirect. L'URL risolto viene cachato su PocketBase collection `excercise`.

```csharp
private async Task<string> ResolveImageUrlAsync(string shortUrl)
{
    using var redirectHttp = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
    redirectHttp.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
    redirectHttp.Timeout = TimeSpan.FromSeconds(5);
    var response = await redirectHttp.GetAsync($"https://{shortUrl}", HttpCompletionOption.ResponseHeadersRead);
    return response.RequestMessage?.RequestUri?.ToString() ?? $"https://{shortUrl}";
}
```

### Tool ExerciseImporter

Il tool console `tools/ExerciseImporter/` serve per pre-caricare le immagini risolte da PC a PocketBase, bypassando il blocco ISP:

```bash
cd tools/ExerciseImporter
dotnet run
```

Il tool:
1. Legge tutti gli ID esercizi dall'API
2. Per ogni esercizio, risolve l'URL immagine
3. Salva nome, bodyPart, equipment, instructions, imageUrl nella collection `excercise` di PocketBase

Eseguire con VPN attiva per evitare il blocco ISP.

## Theme Service

Il tema è gestito programmaticamente senza dipendere da file XAML di palette esterni:

```csharp
public class ThemeService
{
    private readonly Dictionary<string, string> darkPalette = new()
    {
        ["Surface"] = "#0E0E0E",
        ["OnSurface"] = "#E5E2E1",
        ["Primary"] = "#00E5FF",
        // ... 20+ colori
    };

    public void Apply(bool isDark)
    {
        var palette = isDark ? darkPalette : lightPalette;
        foreach (var (key, hex) in palette)
            Application.Current.Resources[key] = Color.FromArgb(hex);
        Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }
}
```

**Vantaggi rispetto a MergedDictionaries swap:**
- I `DynamicResource` vengono notificati automaticamente al cambio valore
- Nessuna dipendenza da URI di file XAML a runtime
- Funziona anche dopo AOT compilation
