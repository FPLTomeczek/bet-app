# apps/web — frontend (Next.js)

@AGENTS.md

Zasady ogólne projektu: patrz `CLAUDE.md` w roocie monorepo.

## Stack
- Next.js 16 (App Router), React 19, TypeScript, Tailwind CSS 4
- Backend: `apps/api` — patrz `apps/api/CLAUDE.md`

## Uwaga o wersji Next.js
Zaimportowany wyżej `AGENTS.md` ostrzega, że **ta wersja Next.js różni się od danych treningowych modelu**. Przed pisaniem kodu opartego o API frameworka sprawdź `node_modules/next/dist/docs/` zamiast polegać na pamięci.

## Typy z API
Typy DTO **generujemy z OpenAPI**, nie przepisujemy ręcznie (patrz „Kontrakt API ↔ front" w roocie). Ręczna kopia typu rozjeżdża się po cichu — wygenerowany typ psuje build, gdy backend zmieni kontrakt.

## Uruchomienie
```bash
npm --prefix apps/web install
npm --prefix apps/web run dev     # http://localhost:3000
```
API musi działać osobno (`http://localhost:5075`), razem z Postgresem z `docker compose`.
