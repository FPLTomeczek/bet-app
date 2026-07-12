# Projekt: aplikacja bukmacherska (projekt do nauki)

## Cel projektu i sposób pracy
- To **projekt edukacyjny**. Celem NIE jest samo dowiezienie działającej aplikacji, lecz **wyciągnięcie maksimum wiedzy technicznej** — zrozumienie *dlaczego*, nie tylko *jak*.
- Zawsze **wyjaśniaj uzasadnienie architektoniczne PRZED szczegółami implementacji**. Pokazuj trade-offy.
- **Preferuję krytyczny feedback nad potakiwaniem.** Kwestionuj moje decyzje, gdy coś się nie spina. Jeśli prowadzenie jest zbyt „za rączkę" (hand-holding) albo za mało rygorystyczne — powiedz to wprost.
- Odpowiadaj **po polsku** (terminy techniczne mogą zostać po angielsku).
- ewentualne komentarze w kodzie w języku angielskim

## Architektura warstw
- **Cienki kontroler** (thin controller) → `DbContext` bezpośrednio jest OK **dla czystego CRUD-u** (brak reguł biznesowych, walidacja to najwyżej „czy FK istnieje").
- **Gdy pojawia się logika biznesowa** (obliczenia, reguły domenowe, koordynacja wielu encji, np. liczenie kursów/wypłaty kuponu, uzgadnianie salda, hashowanie hasła) — wydziel ją do klasy w folderze `Services/` i wstrzykuj do kontrolera przez DI (`AddScoped`). Kontroler zostaje tłumaczem HTTP ↔ domena.
- **Serwis NIE zna HTTP** (żadnego `ModelState`/`ActionResult`). Błędy walidacji zwraca w typie domenowym `Result<T>` (`Services/Result.cs`); kontroler mapuje je na `ValidationProblem`.
- **Nie zakładaj serwisów „na zapas".** Dodawaj je tam, gdzie logika realnie istnieje — nie dla każdej encji z automatu (to zbędny balast). Pierwszy zrealizowany przykład: `CouponService`.

## Stack
- **Baza:** PostgreSQL 17 w Dockerze (docker-compose, named volume)
- **Backend:** .NET SDK 10.x, ASP.NET Core Web API (kontrolery), EF Core, Npgsql — podejście **code-first**
- **Projektowanie schematu:** draw.io (koncept)
- **Klient DB:** DBeaver
- **Sekrety:** .NET User Secrets
- **Środowisko:** WSL2 + Docker Desktop