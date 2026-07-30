# Projekt: aplikacja bukmacherska (projekt do nauki)

Monorepo: backend (.NET) + frontend (Next.js) w jednym repo.

## Cel projektu i sposób pracy
- To **projekt edukacyjny**. Celem NIE jest samo dowiezienie działającej aplikacji, lecz **wyciągnięcie maksimum wiedzy technicznej** — zrozumienie *dlaczego*, nie tylko *jak*.
- Zawsze **wyjaśniaj uzasadnienie architektoniczne PRZED szczegółami implementacji**. Pokazuj trade-offy.
- **Preferuję krytyczny feedback nad potakiwaniem.** Kwestionuj moje decyzje, gdy coś się nie spina. Jeśli prowadzenie jest zbyt „za rączkę" (hand-holding) albo za mało rygorystyczne — powiedz to wprost.
- Odpowiadaj **po polsku** (terminy techniczne mogą zostać po angielsku).
- Ewentualne komentarze w kodzie w języku angielskim.
- Jeśli kod wymaga napisania testów, to je wygeneruj.

## Mapa repo
```
apps/api/    ASP.NET Core Web API + EF Core + PostgreSQL   → apps/api/CLAUDE.md
apps/web/    Next.js 16 + React 19 + Tailwind 4            → apps/web/CLAUDE.md
docker-compose.yml   PostgreSQL 17 (named volume)
```
Reguły specyficzne dla warstwy trzymamy w jej `CLAUDE.md`, nie tutaj.

## Dlaczego monorepo
Najczęstsza zmiana w tym projekcie to **zmiana kontraktu**: nowe pole w DTO po stronie API i jego konsumpcja we froncie. W monorepo to **jeden commit**, który jest spójny albo nie kompiluje się wcale. W dwóch repo istniałoby okno, w którym `main` frontu nie działa z `main` backendu — a niespójność wychodziłaby dopiero w runtime.

Konsekwencja praktyczna: **zmiana API i jej użycie we froncie idą w jednym commicie.**

## Kontrakt API ↔ front
- **API jest jedynym źródłem prawdy** dla kształtu danych. W trybie Development wystawia OpenAPI:
  - dokument: `http://localhost:5075/openapi/v1.json`
  - UI (Scalar): `http://localhost:5075/scalar`
- Front **nie przepisuje typów ręcznie** — generuje je z tego dokumentu. Ręcznie przepisany interfejs TS to kopia prawdy, która cicho się rozjeżdża; wygenerowany typ psuje build, gdy kontrakt się zmieni. O to nam chodzi.
- **Generacja:** `npm --prefix apps/web run gen:api` (API musi działać) → `apps/web/app/lib/api-types.ts`. Plik jest **commitowany** — dzięki temu repo buduje się bez żywego API, a diff w PR pokazuje każdą zmianę kontraktu. Front sięga po typy przez `apps/web/app/lib/api.ts`.
- Enumy jadą po HTTP jako **string** dzięki `[JsonConverter(typeof(JsonStringEnumConverter<T>))]` na samym typie enuma (w modelach) — atrybut widzi zarówno serializer, jak i generator schematu OpenAPI, więc wire format i kontrakt się nie rozjeżdżają. (Globalna rejestracja konwertera by tego nie dała — schemat raportowałby `integer`.)

## Konfiguracja i sekrety

Nowa wartość konfigurowalna → najpierw zaklasyfikuj, potem umieść:

| Kategoria | Kryterium | Gdzie żyje | Przykłady |
|---|---|---|---|
| **Sekret** | ujawnienie = incydent | User Secrets (API) / `.env.local` (web) | hasło Postgresa, przyszły klucz JWT |
| **Config środowiskowy** | nie tajny, ale **różni się** dev/prod | `.env*`, nadpisywany zmienną na prod | `API_BASE_URL`, `POSTGRES_*` |
| **Stała aplikacji** | taka sama **wszędzie** | zwykły kod | `STATUS_LABELS`, `revalidate = 30` |

Trzeci wiersz to ten, na którym się wykłada — nie przenosić stałych do env. `revalidate` w ogóle nie może tam trafić (Next wymaga literału w pliku segmentu).

**API nie dostaje `.env` — sekrety idą do User Secrets** (leżą poza repo, więc nie da się ich zacommitować). Nie zakładać tam `.env`.

## Uruchomienie (dwa procesy)
```bash
# 0. Konfiguracja — raz po clone
cp .env.example .env                      # dane logowania Postgresa (dla compose)
cp apps/web/.env.example apps/web/.env.local
dotnet user-secrets set "ConnectionStrings:BetApp" \
  "Host=localhost;Port=5432;Database=bet-app;Username=<user>;Password=<pass>" \
  --project apps/api/BetApp.Api           # musi zgadzać się z .env

docker compose up -d                      # 1. Postgres
dotnet run --project apps/api/BetApp.Api  # 2. API  → http://localhost:5075
npm --prefix apps/web run dev             # 3. Web  → http://localhost:3000
```

> ⚠️ **Zmiana `POSTGRES_*` w istniejącej instalacji nic nie zmienia w bazie.** Te zmienne działają tylko przy **pierwszej** inicjalizacji pustego katalogu danych, a wolumen `pgdata` jest już zainicjalizowany. Zmiana hasła w `.env` jedynie rozjedzie je z connection stringiem i API przestanie się logować. Świadoma zmiana wymaga `docker compose down -v` — co **usuwa dane**; seed odtworzy zestaw dev.

## Znane luki
- **Brak uwierzytelniania** — `AppUser` istnieje, ale nie ma logowania ani autoryzacji.

Rozwiązane przy pierwszym spięciu front↔API:
- ~~**Brak CORS**~~ — polityka `DevFrontend` (tylko Development, origin `http://localhost:3000`) w `Program.cs`.
- ~~**`UseHttpsRedirection` po HTTP**~~ — teraz aktywne tylko poza Development; w Development front gada po HTTP bez redirectu, który psułby CORS.
