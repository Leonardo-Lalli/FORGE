"""FCM dispatcher. Credentials stay in the LXC, never in the app or repository."""
import os, time, json, urllib.request, urllib.error
import firebase_admin
from firebase_admin import credentials, messaging

firebase_admin.initialize_app(credentials.Certificate(os.environ['GOOGLE_APPLICATION_CREDENTIALS']))
base = os.environ.get('FORGE_INTERNAL_API', 'http://127.0.0.1:8080/api').rstrip('/')
headers = {'X-Forge-Worker': os.environ['FORGE_PUSH_WORKER_TOKEN'], 'Content-Type': 'application/json'}

def request(path, data=None):
    req = urllib.request.Request(base + path, headers=headers, data=None if data is None else json.dumps(data).encode())
    with urllib.request.urlopen(req, timeout=20) as response: return json.load(response)

while True:
    try:
        for item in request('/forge/push/pending')['items']:
            tokens = item['tokens']
            if not tokens: continue  # Keep queued until a device is registered.
            ok = True
            for token in tokens:
                try:
                    messaging.send(messaging.Message(token=token, notification=messaging.Notification(title=item['title'], body=item['body']),
                        data={'route': item['route']}, android=messaging.AndroidConfig(priority='high', notification=messaging.AndroidNotification(channel_id='forge_activity', icon='forge_icon_monochrome'))))
                except messaging.UnregisteredError:
                    pass  # Token is no longer deliverable; other devices can still receive.
                except Exception:
                    ok = False
            request('/forge/push/result', {'id': item['id'], 'sent': ok})
    except (OSError, ValueError, KeyError):
        print('Push delivery unavailable; will retry.', flush=True)
    time.sleep(30)
