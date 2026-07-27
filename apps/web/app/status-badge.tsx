import type { EventResponse } from "./lib/api";
import { STATUS_STYLES, STATUS_LABELS } from "./constants";

// Design-system candidate: a generic status badge. When we adopt a design system this
// moves to the shared UI layer; STATUS_LABELS (domain wording) stays app-side.
export function StatusBadge({ status }: { status: EventResponse["status"] }) {
  return (
    <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${STATUS_STYLES[status]}`}>
      {STATUS_LABELS[status]}
    </span>
  );
}
