# Push Notifications — FORGE

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

### 3. MAUI App Receiver

#### AndroidManifest.xml — aggiungere:

```xml
<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver" 
          android:exported="false" />
<receiver android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver" 
          android:exported="true"
          android:permission="com.google.android.c2dm.permission.SEND">
    <intent-filter>
        <action android:name="com.google.android.c2dm.intent.RECEIVE" />
        <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
        <category android:name="com.companyname.gymtracker.mobile" />
    </intent-filter>
</receiver>
```

#### FirebaseMessagingService.cs (Android native):

```csharp
[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class ForgeFirebaseService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        // Save token to PocketBase user record
        Preferences.Set("fcm_token", token);
        _ = UpdateFcmTokenOnPocketBase(token);
    }
    
    public override void OnMessageReceived(RemoteMessage message)
    {
        var title = message.GetNotification()?.Title ?? "FORGE";
        var body = message.GetNotification()?.Body ?? "";
        
        // Show local notification
        var builder = new NotificationCompat.Builder(this, "forge_channel")
            .SetContentTitle(title)
            .SetContentText(body)
            .SetSmallIcon(Resource.Drawable.ic_notification)
            .SetAutoCancel(true);
        
        var manager = GetSystemService(NotificationService) as NotificationManager;
        manager?.Notify(new Random().Next(), builder.Build());
    }
}
```

#### NuGet packages richiesti:
- `Xamarin.Firebase.Messaging` (per `FirebaseMessagingService`)
- `Xamarin.GooglePlayServices.Base`

### Setup semplificato (senza server intermedio)

Se non vuoi mantenere un server intermedio, puoi usare **PocketBase + Firebase Cloud Functions**:

1. PocketBase hook chiama una Firebase Cloud Function HTTP
2. La Cloud Function invia la notifica FCM
3. Vantaggio: serverless, nessuna manutenzione

## Costi

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
