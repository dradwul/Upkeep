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

The dashboard seeds Swedish car and home examples on first launch. Category templates create common subcategories (such as oil changes, service, and grouped home surfaces), and each subcategory can store dated maintenance events. Data is stored in browser local storage; **Rensa exempeldata** removes sample records without deleting custom entries.

Run locally with `dotnet run --project Upkeep/Upkeep.csproj`.

## GitHub Pages

The repository is `dradwul/Upkeep`; its Pages URL is `https://dradwul.github.io/Upkeep/`. Select **GitHub Actions** as the Pages build and deployment source in Settings > Pages. The `Deploy to GitHub Pages` workflow publishes the app whenever changes are pushed to `main` (or when started manually), and sets the correct base path for both project sites and `dradwul.github.io`.
