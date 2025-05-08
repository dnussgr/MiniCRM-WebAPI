# MiniCRM Web API

## Übersicht

MiniCRM ist eine modulare RESTful Web API für Kunden- und Bestellverwaltung, entwickelt in ASP.NET Core (.NET 9) unter Verwendung von Entity Framework Core und SQLite. Die API implementiert gängige Softwareentwicklungsprinzipien, darunter DTOs, AutoMapper, Soft Deletes sowie Logging und globale Fehlerbehandlung.

---

## Features

- Kunden- und Bestellmanagement via REST-API
- Datenbankzugriff über Entity Framework Core (Code First)
- Soft Deletes mit Zeitstempel
- DSGVO-konforme Anonymisierung von Kundendaten
- DTOs für klare Trennung von API- und Datenbankmodellen
- AutoMapper zur effizienten Objektabbildung
- Unit Tests mit xUnit, Moq und InMemory-Datenbank
- Logging mittels Serilog (Konsole + Rolling File)
- Swagger/OpenAPI-Integration zur API-Dokumentation

---

## Technologien

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- AutoMapper
- Serilog
- xUnit & Moq
- Swagger / Swashbuckle

---

## Projektstruktur
```
MiniCRM-WebAPI/
│
├── Controllers/
│ ├── CustomerController.cs
│ └── OrderController.cs
│
├── Data/
│ └── AppDbContext.cs
│
├── Dtos/
│ └── Create/Update/Read DTOs
│
├── Models/
│ ├── Customer.cs
│ └── Order.cs
│
├── Middleware/
│ └── ExceptionHandlingMiddleware.cs
│
├── Profiles/
│ └── MappingProfile.cs
│
├── Program.cs
├── appsettings.json
├── MiniCRM.Tests/
│ └── Unit-Tests mit xUnit
```

---

## Setup und Ausführung

### Voraussetzungen

- Visual Studio 2022 oder höher
- .NET 9 SDK

### Starten der API

1. Repository klonen
2. Datenbankmigration anwenden:
```
dotnet ef database update
```
3. Projekt ausführen
```
dotnet run
```
4. Swagger UI aufrufen unter:
```
https://localhost:{PORT}/swagger
```

---

## Tests

Die Unit Tests befinden sich im Projektordner `MiniCRM.Tests`.

```
dotnet test
```
## Weiterentwicklung (geplant)
- Integrationstests mit HTTP-Testserver
- Pagination bei Abfragen
- Rollenbasierte Authentifizierung
- Frontend-Integration
