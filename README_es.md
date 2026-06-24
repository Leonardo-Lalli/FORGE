<table style="width:100%;border-collapse:collapse;margin-bottom:16px"><tr><td style="background-color:#dc2626;color:white;padding:16px;text-align:center;font-weight:bold;font-size:16px;border-radius:8px">&#x26A0; SERVER OFFLINE &#x26A0; &mdash; Il server FORGE &egrave; temporaneamente irraggiungibile. Torner&agrave; online al pi&ugrave; presto.</td></tr></table>
<div align="center">

> ⚠️ **PROYECTO AMATEUR CON FINES EDUCATIVOS** ⚠️  
> Esta app está desarrollada por un estudiante como proyecto escolar.  
> **No es un producto comercial.** El servidor está alojado en hardware doméstico.  
> Puede contener errores, limitaciones y tiempos de inactividad no planificados.  
> Úsala para pruebas y demostraciones, no para datos importantes.

[🇬🇧 English](README_en.md) · [🇮🇹 Italiano](README.md) · [🇪🇸 Español](README_es.md) · [🇨🇳 中文](README_zh.md)

<img src="assets/screenshots/dashboard.jpeg" width="120" style="border-radius:24px" />

# FORGE

### El Diario de Entrenamiento Social

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/maui/)
[![PocketBase](https://img.shields.io/badge/PocketBase-Backend-000000?logo=pocketbase)](https://pocketbase.io/)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1.500%2B%20ejercicios-22C55E)](https://oss.exercisedb.dev)
[![MVVM](https://img.shields.io/badge/Arquitectura-MVVM%20%7C%20CommunityToolkit-7C3AED)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Tests](https://img.shields.io/badge/Tests-27%20aprobados-brightgreen)](tests/)
[![License](https://img.shields.io/badge/Licencia-MIT-blue)](LICENSE)

**Convierte cada repetición en progreso. Desafía a tus amigos. Supera tus límites.**

</div>

---

## Qué es FORGE

FORGE es una app Android para registrar entrenamientos de gimnasio con funciones sociales. Registra ejercicios, series y repeticiones. Sigue a tus amigos. Desbloquea logros. Observa tu progreso con estadísticas detalladas.

---

## Funcionalidades

| Categoría | Funcionalidad |
|-----------|--------------|
| 🏋️ **Entrenamiento** | Búsqueda de ejercicios (1.500+ con GIFs), series kg×reps, checkmark, temporizador, fotos, minimizar/borrador |
| 📊 **Estadísticas** | Gráfico de volumen, top lifts, calendario mensual, filtros SEMANA/MES/3M/AÑO/TODO |
| 👥 **Social** | Feed de amigos, like ♥, seguir/dejar de seguir, búsqueda de usuarios, solicitudes |
| 🏆 **Logros** | 48 insignias para desbloquear, seguimiento automático, vitrina en perfil |
| 👤 **Perfil** | Avatar, bio, estadísticas, historial, insignias desbloqueadas |
| 🎨 **Interfaz** | Tema dual claro/oscuro, fuentes Inter/Lexend/Space Grotesk |
| 📱 **Offline** | SQLite local con sincronización automática |
| 📁 **CSV** | Importar/exportar entrenamientos |
| 🔒 **Seguridad** | SecureStorage para contraseñas, HTTPS, reglas API row-level, panel admin bloqueado |

---

## Arquitectura

| Capa | Tecnología |
|------|-----------|
| Framework | .NET MAUI 10 (Android-first) |
| UI | MVVM con CommunityToolkit.Mvvm 8.4 |
| Navegación | Shell (3 pestañas + 8 rutas) |
| Backend | PocketBase auto-alojado |
| API | ExerciseDB v1 (1.500+ ejercicios, gratis) |
| Persistencia | SQLite (sqlite-net-pcl) |
| Tests | xUnit (27 tests) |
| Fuentes | Inter, Lexend, Space Grotesk |

---

## Desarrollo

```bash
git clone https://github.com/USERNAME/FORGE.git
cd FORGE
cp .env.example .env  # editar con tu URL de PocketBase
dotnet build src/Forge/Forge.csproj -f net10.0-android
dotnet test tests/Forge.Tests/
```

---

## Privacidad · Seguridad · Aviso Legal

**Privacidad**: Los datos se guardan en tu teléfono (SQLite) y en el servidor FORGE (PocketBase). Las fotos se guardan como parte del entrenamiento.

**Seguridad**: Contraseñas cifradas (SecureStorage), HTTPS, reglas API row-level, panel admin bloqueado, rate limiting.

**Aviso**: FORGE es un proyecto educativo. No es un producto comercial. El servidor es casero y puede no estar siempre disponible.

**Licencia**: MIT — ver [LICENSE](LICENSE).

---

<div align="center">

**FORGE** — Construye tu físico. Desafía a tus amigos. Forja tu leyenda.

</div>
