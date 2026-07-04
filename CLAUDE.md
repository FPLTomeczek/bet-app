# Projekt: aplikacja bukmacherska (projekt do nauki)

## Cel projektu i sposób pracy
- To **projekt edukacyjny**. Celem NIE jest samo dowiezienie działającej aplikacji, lecz **wyciągnięcie maksimum wiedzy technicznej** — zrozumienie *dlaczego*, nie tylko *jak*.
- Zawsze **wyjaśniaj uzasadnienie architektoniczne PRZED szczegółami implementacji**. Pokazuj trade-offy.
- **Preferuję krytyczny feedback nad potakiwaniem.** Kwestionuj moje decyzje, gdy coś się nie spina. Jeśli prowadzenie jest zbyt „za rączkę" (hand-holding) albo za mało rygorystyczne — powiedz to wprost.
- Odpowiadaj **po polsku** (terminy techniczne mogą zostać po angielsku).
- ewentualne komentarze w kodzie w języku angielskim

## Stack
- **Baza:** PostgreSQL 17 w Dockerze (docker-compose, named volume)
- **Backend:** .NET SDK 10.x, ASP.NET Core Web API (kontrolery), EF Core, Npgsql — podejście **code-first**
- **Projektowanie schematu:** draw.io (koncept)
- **Klient DB:** DBeaver
- **Sekrety:** .NET User Secrets
- **Środowisko:** WSL2 + Docker Desktop