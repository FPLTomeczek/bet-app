import type { EventResponse } from "./lib/api";

// Keyed by the contract's status union, so adding or dropping a status is a compile error.
export const STATUS_STYLES: Record<EventResponse["status"], string> = {
  Scheduled: "bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300",
  Live: "bg-red-100 text-red-700 dark:bg-red-950 dark:text-red-300",
  Finished: "bg-green-100 text-green-700 dark:bg-green-950 dark:text-green-300",
  Cancelled: "bg-amber-100 text-amber-700 dark:bg-amber-950 dark:text-amber-300",
};

export const STATUS_LABELS: Record<EventResponse["status"], string> = {
  Scheduled: "Zaplanowane",
  Live: "Na żywo",
  Finished: "Zakończone",
  Cancelled: "Odwołane",
};
