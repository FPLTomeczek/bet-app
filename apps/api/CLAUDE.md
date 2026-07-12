# apps/api — backend (ASP.NET Core)

Zasady ogólne projektu: patrz `CLAUDE.md` w roocie monorepo.

## Architektura warstw
- **Cienki kontroler** (thin controller) → `DbContext` bezpośrednio jest OK **dla czystego CRUD-u** (brak reguł biznesowych, walidacja to najwyżej „czy FK istnieje").
- **Gdy pojawia się logika biznesowa** (obliczenia, reguły domenowe, koordynacja wielu encji, np. liczenie kursów/wypłaty kuponu, uzgadnianie salda, hashowanie hasła) — wydziel ją do klasy w folderze `Services/` i wstrzykuj do kontrolera przez DI (`AddScoped`). Kontroler zostaje tłumaczem HTTP ↔ domena.
- **Serwis NIE zna HTTP** (żadnego `ModelState`/`ActionResult`). Błędy walidacji zwraca w typie domenowym `Result<T>` (`Services/Result.cs`); kontroler mapuje je na `ValidationProblem`.
- **Nie zakładaj serwisów „na zapas".** Dodawaj je tam, gdzie logika realnie istnieje — nie dla każdej encji z automatu (to zbędny balast). Pierwszy zrealizowany przykład: `CouponService`.

## Stack
- **Baza:** PostgreSQL 17 w Dockerze (docker-compose w roocie, named volume)
- **Backend:** .NET SDK 10.x, ASP.NET Core Web API (kontrolery), EF Core, Npgsql — podejście **code-first**
- **Projektowanie schematu:** draw.io (koncept)
- **Klient DB:** DBeaver
- **Sekrety:** .NET User Secrets

## Uruchomienie
```bash
docker compose up -d              # z roota monorepo
dotnet run --project apps/api/BetApp.Api
dotnet test  apps/api/bet-app.slnx
```

## Kontrakt z frontem
API jest **jedynym źródłem prawdy** dla kształtu danych. Front (`apps/web`) generuje swoje typy TS ze schematu OpenAPI — nie przepisuje ich ręcznie.
Zmieniasz DTO → w tym samym commicie zregeneruj typy frontu. Szczegóły: `CLAUDE.md` w roocie.
