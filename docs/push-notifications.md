# Push Notifications — FORGE

## ⚠️ Stato attuale (.NET 10)

I pacchetti NuGet `Xamarin.Firebase.Messaging` non supportano .NET 10 (richiedono net8.0-android).
Pertanto l'SDK Firebase **non può essere integrato direttamente nell'app MAUI** al momento.

**Soluzione implementata**: PocketBase hook lato server per invio FCM + polling lato app come fallback.

## Panoramica

Le notifiche push permettono di ricevere alert sul telefono (anche ad app chiusa) quando:
- Un amico mette like a un tuo allenamento
- Un utente ti invia una richiesta di follow
- Un amico accetta la tua richiesta di follow

## Architettura

```
PocketBase ───hook──▶ Server Intermedio ───FCM──▶ Google Firebase ───▶ Telefono Android
  (evento)            (Go/Node/C#)              (gratuito)            (notifica nativa)
```

## Setup — 3 componenti

### 1. Firebase Project (Google Cloud Console)

1. Vai su https://console.firebase.google.com/
2. Crea nuovo progetto: `forge-fitness`
3. Aggiungi app Android: pacchetto `com.companyname.gymtracker.mobile`
4. Scarica `google-services.json`
5. Copialo in `src/GymTracker.Mobile/Platforms/Android/google-services.json`
6. Imposta come `GoogleServicesJson` nel `.csproj`

### 2. PocketBase Hook (Server Intermedio)

Opzione A: **Hook Go** (integrato in PocketBase)

Crea un file `pb_hooks/push.go` nella directory PocketBase:

```go
package main

import (
    "bytes"
    "encoding/json"
    "net/http"
    "github.com/pocketbase/pocketbase/core"
)

func registerHooks(app core.App) {
    // Quando un workout riceve un like
    app.OnRecordAfterUpdateRequest("logged_workouts").Add(func(e *core.RecordUpdateEvent) error {
        // Check if likes increased
        oldLikes := e.Record.Original().GetInt("likes")
        newLikes := e.Record.GetInt("likes")
        if newLikes <= oldLikes { return nil }
        
        // Get workout owner
        ownerId := e.Record.GetString("user")
        // Send FCM to workout owner
        sendFCM(ownerId, "Nuovo like!", "Qualcuno ha messo like al tuo allenamento")
        return nil
    })
    
    // Quando arriva una richiesta di follow
    app.OnRecordAfterCreateRequest("social_graph").Add(func(e *core.RecordCreateEvent) error {
        toUser := e.Record.GetString("to_user")
        fromName := e.Record.GetString("from_name")
        sendFCM(toUser, "Nuovo follower!", fromName + " vuole seguirti")
        return nil
    })
}

func sendFCM(userId, title, body string) {
    // Get FCM token from users collection (field: fcm_token)
    // POST to https://fcm.googleapis.com/fcm/send
    // with Server Key from Firebase Console
}
```

Opzione B: **Server esterno C#** (più facile da debuggare)

```csharp
// Minimal FCM relay server (ASP.NET Core)
app.MapPost("/push/send", async (FcmRequest req) =>
{
    var fcm = FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance;
    await fcm.SendAsync(new Message
    {
        Token = req.Token,
        Notification = new Notification { Title = req.Title, Body = req.Body }
    });
});
```

### 3. MAUI App — Polling (fallback .NET 10)

Finché i pacchetti Firebase non supporteranno .NET 10, l'app usa **polling**:
- Ogni 60 secondi controlla PocketBase per nuove notifiche
- Mostra un badge sul tab Stats
- Apre FriendRequestsPage se ci sono novità

Il polling è già implementato (FriendRequestsPage si aggiorna su `OnAppearing()`).

Quando Firebase SDK supporterà .NET 10, aggiungere:
1. `Xamarin.Firebase.Messaging` NuGet
2. `ForgeFirebaseService.cs` (FirebaseMessagingService Android)
3. `google-services.json` da Firebase Console
4. Campo `fcm_token` nella collection `users` di PocketBase

## Setup Rapido (oggi)

| Componente | Costo |
|-----------|-------|
| Firebase Project | Gratuito |
| FCM | Gratuito (illimitato) |
| Cloud Function | Gratuito (2M invocazioni/mese) |
| PocketBase Hook | Host sul tuo server PocketBase |

## Ordine di implementazione consigliato

1. Creare progetto Firebase + scaricare google-services.json
2. Aggiungere `FirebaseMessagingService` nell'app MAUI
3. Aggiungere campo `fcm_token` nella collection `users` di PocketBase
4. Salvare il token FCM nel record utente al login
5. Creare PocketBase hook per inviare notifiche
6. Test: mettere like a un workout e verificare che arrivi la notifica

## Note

- Le notifiche funzionano solo su dispositivo fisico (non su emulatore)
- Il token FCM va aggiornato ogni volta che l'app viene reinstallata
- PocketBase non ha un sistema di webhook nativo — serve un hook custom o un server intermedio
- Alternativa più semplice: **polling periodico** (ogni 60s) invece di push vere. Meno professionale ma zero setup.
