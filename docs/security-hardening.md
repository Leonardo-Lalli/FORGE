# PocketBase Security Hardening

## 1. API Rules — Row-Level Ownership

Sostituisci le regole attuali con queste (Admin UI → Collections → Edit → API Rules):

### logged_workouts

```
List/Search: @request.auth.id != "" && user = @request.auth.id
View:        @request.auth.id != ""
Create:      @request.auth.id != "" && user = @request.auth.id
Update:      @request.auth.id != "" && (user = @request.auth.id || @request.auth.id ?= liked_by)
Delete:      @request.auth.id != "" && user = @request.auth.id
```

### social_graph

```
List/Search: @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
View:        @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
Create:      @request.auth.id != "" && from_user = @request.auth.id
Update:      @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
Delete:      @request.auth.id != "" && (from_user = @request.auth.id || to_user = @request.auth.id)
```

### excercise (catalogo pubblico)

```
List/Search: @request.auth.id != ""
View:        @request.auth.id != ""
Create:      @request.auth.id != ""
Update:      @request.auth.id = ""         (solo admin/superuser)
Delete:      @request.auth.id = ""         (solo admin/superuser)
```

### users

```
List/Search: @request.auth.id != ""
View:        @request.auth.id != ""
Update:      @request.auth.id != "" && id = @request.auth.id
Delete:      @request.auth.id = ""         (solo admin/superuser)
```

## 2. Nginx — Bloccare Admin Panel

Aggiungi questo al file di configurazione Nginx (Proxy Manager o &lt;NGINX_PATH&gt;/):

```nginx
# Blocca PocketBase Admin Panel da accesso esterno
location /_/ {
    deny all;
    return 403;
}

# Rate limiting per auth endpoint
limit_req_zone $binary_remote_addr zone=auth:10m rate=5r/m;

location /api/collections/users/auth-with-password {
    limit_req zone=auth burst=3 nodelay;
    proxy_pass http://pocketbase:8090;
}

# Rate limiting per API generica
limit_req_zone $binary_remote_addr zone=api:10m rate=60r/m;

location /api/ {
    limit_req zone=api burst=20 nodelay;
    proxy_pass http://pocketbase:8090;
}

# Security headers globali
add_header Strict-Transport-Security "max-age=63072000; includeSubDomains" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-Frame-Options "DENY" always;
add_header X-XSS-Protection "1; mode=block" always;

# Limite dimensione upload (foto, avatar)
client_max_body_size 5m;

# Nascondi versione server
server_tokens off;
```

## 3. PocketBase Config

Assicurati che PocketBase sia configurato solo su HTTPS:

```bash
# &lt;PB_INSTALL_PATH&gt;/pocketbase serve --https=0.0.0.0:8090
# NON usare --http in produzione
```

Se la tua istanza PocketBase esponeva HTTP, blocca la porta nel firewall:

```bash
# Su Proxmox/Debian:
ufw deny 8080
# o con iptables:
iptables -A INPUT -p tcp --dport 8080 -j DROP
```
