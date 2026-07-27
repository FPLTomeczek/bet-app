import type { EventResponse } from "./lib/api";
import { formatEventStart } from "./helpers";
import { StatusBadge } from "./status-badge";

export function EventRow({ event }: { event: EventResponse }) {
  return (
    <li className="flex items-center justify-between gap-4 rounded-lg border border-black/[.08] px-4 py-3 dark:border-white/[.12]">
      <div className="min-w-0">
        <p className="truncate font-medium">{event.name}</p>
        <p className="text-sm text-zinc-500">{formatEventStart(event.startTime)}</p>
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
