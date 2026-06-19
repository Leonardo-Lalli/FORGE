# Contributing to FORGE

Grazie per voler contribuire! Ecco come puoi aiutare.

## Come contribuire

1. **Forka** la repo
2. Crea un branch: `feature/nome-funzionalita` o `fix/nome-bug`
3. Fai le modifiche
4. Assicurati che i test passino: `dotnet test tests/Forge.Tests/`
5. Apri una Pull Request

## Linee guida

- Segui il pattern MVVM esistente (niente logica nei code-behind)
- Usa `CommunityToolkit.Mvvm` per ObservableProperty e RelayCommand
- Aggiungi stati UI (loading, error, empty, success) in ogni ViewModel
- Scrivi test per model e converter
- Non introdurre nuovi pacchetti NuGet senza discuterne

## Setup sviluppo

```bash
git clone https://github.com/Leonardo-Lalli/FORGE.git
cd FORGE
cp .env.example .env
# Modifica .env con l'URL del tuo server PocketBase
dotnet build src/Forge/Forge.csproj -f net10.0-android
dotnet test tests/Forge.Tests/
```

## Struttura

```
src/Forge/
├── Models/          # Entità dominio + DTO
├── ViewModels/      # MVVM (CommunityToolkit)
├── Views/           # XAML (nessuna logica)
├── Services/        # Business logic
├── Converters/      # IValueConverter
└── Resources/       # Stili, font, immagini
```

## Domande?

Apri una [Discussion](https://github.com/Leonardo-Lalli/FORGE/discussions).
