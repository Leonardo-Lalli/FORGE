// PocketBase Hook — Push Notifications via Firebase Cloud Messaging
// Posiziona questo file in: /pb/pb_hooks/push.pb.js (nel container Docker)
// PocketBase 0.22+ usa JavaScript hooks con estensione .pb.js
//
// Setup:
// 1. Sostituisci FCM_SERVER_KEY con la Server Key da Firebase Console
//    (Project Settings → Cloud Messaging → Server Key)
// 2. Assicurati che la collection "users" abbia un campo "fcm_token" (Plain text, opzionale)
// 3. Riavvia PocketBase per caricare l'hook

const FCM_SERVER_KEY = "YOUR_FCM_SERVER_KEY_HERE";

// Invia notifica FCM a un utente
function sendFcm(token, title, body, clickAction) {
    if (!token || token === "") return;
    try {
        const payload = {
            to: token,
            notification: {
                title: title || "FORGE",
                body: body || "",
                sound: "default",
                click_action: clickAction || "OPEN_APP"
            },
            data: {
                click_action: clickAction || "OPEN_APP"
            }
        };
        const resp = $http.request({
            method: "POST",
            url: "https://fcm.googleapis.com/fcm/send",
            body: JSON.stringify(payload),
            headers: {
                "Content-Type": "application/json",
                "Authorization": "key=" + FCM_SERVER_KEY
            },
            timeout: 5000
        });
        console.log("[FCM] sent to " + token.substring(0, 10) + "... status=" + resp.statusCode);
    } catch (err) {
        console.log("[FCM] error: " + err);
    }
}

// Trova il token FCM di un utente
function getUserFcmToken(userId) {
    try {
        const record = $app.findRecordById("users", userId);
        return record ? (record.getString("fcm_token") || "") : "";
    } catch (err) {
        return "";
    }
}

// ═══ EVENT HOOKS ═══

// Nuovo like su un allenamento
onRecordAfterUpdateRequest((e) => {
    if (e.collection.name !== "logged_workouts") return;

    const oldLikes = e.record?.originalCopy?.getInt("likes") || 0;
    const newLikes = e.record?.getInt("likes") || 0;
    if (newLikes <= oldLikes) return;

    const ownerId = e.record.getString("user") || "";
    if (!ownerId) return;

    const token = getUserFcmToken(ownerId);
    const workoutName = e.record.getString("name") || "Allenamento";
    sendFcm(token, "Nuovo like! \u2665", "Qualcuno ha messo like al tuo allenamento \"" + workoutName + "\"");
}, "logged_workouts");

// Nuova richiesta di follow
onRecordAfterCreateRequest((e) => {
    if (e.collection.name !== "social_graph") return;

    const toUser = e.record.getString("to_user") || "";
    const fromName = e.record.getString("from_name") || "Qualcuno";
    if (!toUser) return;

    const token = getUserFcmToken(toUser);
    sendFcm(token, "Nuovo follower!", fromName + " vuole seguirti", "OPEN_FRIEND_REQUESTS");
}, "social_graph");

// Richiesta di follow accettata
onRecordAfterUpdateRequest((e) => {
    if (e.collection.name !== "social_graph") return;

    const status = e.record.getString("status") || "";
    const oldStatus = e.record?.originalCopy?.getString("status") || "";
    if (status !== "accepted" || oldStatus === "accepted") return;

    const fromUser = e.record.getString("from_user") || "";
    const toName = e.record.getString("to_name") || "";
    if (!fromUser) return;

    const token = getUserFcmToken(fromUser);
    sendFcm(token, "Richiesta accettata \u2705", toName + " ha accettato la tua richiesta di follow", "OPEN_FEED");
}, "social_graph");

console.log("[FORGE Hook] Push notifications active. FCM key " + (FCM_SERVER_KEY === "YOUR_FCM_SERVER_KEY_HERE" ? "NOT SET - notifications disabled" : "configured"));
