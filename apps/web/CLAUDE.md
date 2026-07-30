# apps/web — frontend (Next.js)

@AGENTS.md

Zasady ogólne projektu: patrz `CLAUDE.md` w roocie monorepo.

## Stack
- Next.js 16 (App Router), React 19, TypeScript, Tailwind CSS 4
- Backend: `apps/api` — patrz `apps/api/CLAUDE.md`

## Uwaga o wersji Next.js
Zaimportowany wyżej `AGENTS.md` ostrzega, że **ta wersja Next.js różni się od danych treningowych modelu**. Przed pisaniem kodu opartego o API frameworka sprawdź `node_modules/next/dist/docs/` zamiast polegać na pamięci.

## Zmienne środowiskowe
Kontrakt: `.env.example` (commitowany) → `.env.local` (ignorowany). Co jest sekretem, co configiem, a co stałą: „Konfiguracja i sekrety" w roocie.

- **`process.env` czytamy wyłącznie w `app/lib/env.ts`**, reszta importuje `env`.
- **Bez cichych fallbacków** — brakująca zmienna to twardy błąd, także w `next build` (prerender ładuje ten moduł). Nie dopisywać `?? "localhost..."`.
- **Nowa zmienna dla przeglądarki wymaga decyzji, nie prefiksu.** `NEXT_PUBLIC_` wkleja wartość do bundla na etapie builda — powody i pułapki w komentarzu `env.ts`; mechanika w docsach Nexta (patrz uwaga o wersji wyżej).
- **Vitest nie wczytuje plików `.env` Nexta.** Jeśli test potrzebuje zmiennej, to sygnał, że dosięgnął klienta API i należy go zamockować — nie karmić URL-em. W ostateczności `vi.stubEnv` w tym jednym pliku, nie globalny `test.env`.

## Typy z API
Typy DTO **generujemy z OpenAPI**, nie przepisujemy ręcznie (patrz „Kontrakt API ↔ front" w roocie). Ręczna kopia typu rozjeżdża się po cichu — wygenerowany typ psuje build, gdy backend zmieni kontrakt.

## Struktura folderu trasy
Dziel kod **modułowo**. Moduł = folder z głównym komponentem, a obok — **tylko jeśli potrzebne** — pliki wg roli (żadnych pustych plików „na zapas"):

| Plik | Zawiera |
|---|---|
| `page.tsx` / `<moduł>.tsx` | główny komponent (jedna funkcja); w trasie `page.tsx` + ewentualny config segmentu Next |
| `helpers.ts` | funkcje pomocnicze (formatowanie, transformacje) |
| `constants.ts` | stałe, typy, interfejsy |
| `use-<nazwa>.ts` | hook — jeden plik na hook, nazwany od nazwy hooka (`use-events.ts` → `useEvents`) |
| `<moduł>.test.tsx` | testy modułu |

Reguły i wyjątki:
- **Config segmentu Next (`export const revalidate`, `dynamic`, …) zostaje w `page.tsx`.** Next czyta go przez **statyczną analizę pliku-segmentu** — re-eksport z `constants.ts` nie zadziała. To kontrakt frameworka, nie stała aplikacji.
- **Komponent feature vs prymityw DS:** komponent znający DTO domenowe (`EventRow`) zostaje przy module; generyczny prymityw prezentacyjny (`StatusBadge`) to kandydat do warstwy Design System.

## Testy
**Vitest + React Testing Library** (unit/komponenty). E2E (Playwright) — jeszcze nie ma, dojdzie przy stabilnych flow.
```bash
npm --prefix apps/web test         # watch
npm --prefix apps/web run test:run # jednorazowo (CI)
```
Testy leżą przy module jako `<moduł>.test.tsx` (patrz „Struktura folderu trasy").

Co czym testować:

| Kod | Narzędzie | Dlaczego |
|---|---|---|
| `helpers.ts` (czyste funkcje) | sam Vitest | najwyższy ROI, zero DOM, testujesz logikę nie framework |
| `use-*.ts` (hooki) | Vitest + RTL (`renderHook`) | logika stanu klienta |
| **synchroniczne** komponenty (Server i Client) | Vitest + RTL | renderują się w jsdom |
| **`async` Server Components**, pełne flow | **E2E**, nie unit | RTL/jsdom nie renderują async RSC — [oficjalne zalecenie Next](node_modules/next/dist/docs/01-app/02-guides/testing/vitest.md) |

## Uruchomienie
```bash
npm --prefix apps/web install
npm --prefix apps/web run dev     # http://localhost:3000
```
API musi działać osobno (`http://localhost:5075`), razem z Postgresem z `docker compose`.
