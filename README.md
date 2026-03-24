# BiztvillCRM

Biztovill CRM – SaaS alapú mérés- és hitelesítés-nyilvántartó rendszer

## Technológiai Stack

| Réteg | Technológia |
|---|---|
| Frontend | Blazor Web App (.NET 8+, Interactive Auto) |
| UI Komponensek | MudBlazor (Material Design) |
| Backend | ASP.NET Core 8+ |
| ORM | Entity Framework Core + Pomelo.EntityFrameworkCore.MySql |
| Adatbázis | MySQL 8.0+ |
| Autentikáció | ASP.NET Core Identity + JWT |
| Validáció | FluentValidation |
| PDF generálás | QuestPDF |
| Email | MailKit |
| Háttérfeladatok | BackgroundService |

## Projekt Struktúra

```
BiztvillCRM.sln
├── BiztvillCRM.Web/           ← Szerver + Blazor host
├── BiztvillCRM.Client/        ← WASM kliens
├── BiztvillCRM.Shared/        ← Közös modellek, DTOs, enums
├── BiztvillCRM.Data/          ← EF Core DbContext, Repository
├── BiztvillCRM.Services/      ← Üzleti logika
└── Scripts/MYSQL_NEW/         ← Adatbázis scriptek
```

## Adatbázis

A Scripts/MYSQL_NEW/ mappában található scriptek sorrendben futtatandók (01→09).

## Licensz modell

Hibrid: szintalapú (Próba/Alap/Profi/Vállalati) + modulalapú funkciók.
