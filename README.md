# Upkeep

A Swedish-first personal maintenance and asset logbook built as a Blazor WebAssembly PWA.

## Project structure

```text
Upkeep/
  Models/       Category, SubCategory, and LogEntry domain models
  Pages/        Dashboard and routed pages
  Services/     ILogService and browser local-storage implementation
  Layout/       Shared application layout
  wwwroot/      App shell, theme styles, icons, and PWA assets
```

The dashboard seeds a small Swedish example collection on first launch. Categories and their entries are stored in browser local storage; use **Rensa exempeldata** to remove only the sample categories.

Run locally with `dotnet run --project Upkeep/Upkeep.csproj`.

## GitHub Pages

The `Deploy to GitHub Pages` workflow publishes the app whenever changes are pushed to `main` (or when started manually). Create the repository under `dradwul`, then select **GitHub Actions** as the Pages build and deployment source in the repository's Settings > Pages. The workflow sets the correct base path for both project sites and `dradwul.github.io`.
