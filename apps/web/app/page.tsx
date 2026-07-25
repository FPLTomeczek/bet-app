import { type EventResponse } from "./lib/api";
import { getEvents } from "./lib/queries";

// ISR: cache the rendered page and regenerate at most every 30s. The event list is a
// slow-changing catalogue, so we don't opt into force-dynamic (per-request render).
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

function EventRow({ event }: { event: EventResponse }) {
  const start = new Date(event.startTime).toLocaleString("pl-PL", {
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });

  return (
    <li className="flex items-center justify-between gap-4 rounded-lg border border-black/[.08] px-4 py-3 dark:border-white/[.12]">
      <div className="min-w-0">
        <p className="truncate font-medium">{event.name}</p>
        <p className="text-sm text-zinc-500">{start}</p>
      </div>
      <div className="flex shrink-0 items-center gap-3">
        {event.result && (
          <span className="tabular-nums text-sm font-medium">{event.result}</span>
        )}
        <StatusBadge status={event.status} />
      </div>
    </li>
  );
}

// `status` is the string union from the contract, so these Record lookups are exhaustive
// — dropping a case is a compile error.
function StatusBadge({ status }: { status: EventResponse["status"] }) {
  const styles: Record<EventResponse["status"], string> = {
    Scheduled: "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300",
    Live: "bg-red-100 text-red-700 dark:bg-red-950 dark:text-red-300",
    Finished: "bg-green-100 text-green-700 dark:bg-green-950 dark:text-green-300",
    Cancelled: "bg-amber-100 text-amber-700 dark:bg-amber-950 dark:text-amber-300",
  };

  const labels: Record<EventResponse["status"], string> = {
    Scheduled: "Zaplanowane",
    Live: "Na żywo",
    Finished: "Zakończone",
    Cancelled: "Odwołane",
  };

  return (
    <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${styles[status]}`}>
      {labels[status]}
    </span>
  );
}
