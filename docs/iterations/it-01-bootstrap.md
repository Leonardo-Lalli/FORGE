# IT-01 — Bootstrap MAUI e Design System

**Data:** Inizio progetto
**Branch:** `main` (poi `mockup`)
**Stato:** ✅ Completata

## Obiettivo verificabile

Progetto MAUI compilabile su Android con Shell navigation a 3 tab e tema Stitch applicato con supporto doppio tema (chiaro/scuro).

## In scope

- Creazione progetto `src/GymTracker.Mobile/`
- AppShell con 3 tab: Dashboard, Feed, Stats
- Pagine placeholder per ogni tab
- Design System: colori ciano/primary, dynamic resource per cambio tema
- Font Google Fonts: Inter (body), Lexend (label), Space Grotesk (headline)
- Dependency injection in `MauiProgram.cs`
- `BaseViewModel` con `IsBusy`, `ErrorMessage`, `HasData`, `IsEmptyState`
- `ThemeService` con palette inline e toggle runtime
- Cartelle: `Models/`, `ViewModels/`, `Views/`, `Services/`, `Converters/`
- Build secrets injection: `.env` → `Resources/Raw/gymtracker.env`

## File creati/modificati

| File | Descrizione |
|------|-------------|
| `MauiProgram.cs` | DI composition root, HttpClient PocketBase + ExerciseDB, font registration |
| `App.xaml` / `App.xaml.cs` | Converter globali, CreateWindow con auto-login |
| `AppShell.xaml` / `AppShell.xaml.cs` | 3-tab Shell + 7 route di dettaglio |
| `ViewModels/BaseViewModel.cs` | Base class con `SetLoading()`, `SetSuccess()`, `SetError()` |
| `Services/ThemeService.cs` | Palette dark/light inline, `WriteResources()`, `Apply()` |
| `Services/BuildSecrets.cs` | Caricamento `.env` → dictionary, `ConcurrentDictionary` |
| `Resources/Styles/Styles.xaml` | Stili globali con DynamicResource |
| `Converters/InverseBoolConverter.cs` | Inverse bool per visibilità |
| `Converters/BoolToVisibilityConverter.cs` | Bool → visibilità |

## Criteri di accettazione

- [x] Il progetto compila con `dotnet build` senza errori
- [x] L'app si avvia su emulatore Android e mostra 3 tab
- [x] Navigazione tra tab funzionante
- [x] Tema dark/light applicato con toggle nelle Impostazioni
- [x] DynamicResource su tutte le pagine per cambio tema runtime
- [x] Font Inter, Lexend, Space Grotesk caricati e usati

## Verifiche eseguite

- `dotnet build src/GymTracker.Mobile/GymTracker.Mobile.csproj -f net10.0-android` ✅
- Navigazione manuale 3 tab ✅
- Toggle tema chiaro/scuro nelle Impostazioni ✅
- Cambio tema si propaga a tutte le pagine (Shell inclusa) ✅
