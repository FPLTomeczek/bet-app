import { getEvents } from "./lib/queries";
import { EventRow } from "./event-row";

// Must be a direct export from the route-segment file — Next reads route config by static
// analysis of page.tsx, so it can't move to constants.ts. ISR: cache the page and
// regenerate at most every 30s (the event list is a slow-changing catalogue).
export const revalidate = 30;

export default async function Home() {
  const events = await getEvents();

  return (
    <main className="mx-auto min-h-screen max-w-3xl px-6 py-12 font-sans">
      <h1 className="mb-1 text-2xl font-semibold tracking-tight">Wydarzenia</h1>
      <p className="mb-8 text-sm text-zinc-500">
        {events.length} {events.length === 1 ? "wydarzenie" : "wydarzeń"} z API
      </p>

      <ul className="flex flex-col gap-2">
        {events.map((event) => (
          <EventRow key={event.id} event={event} />
        ))}
      </ul>
    </main>
  );
}
