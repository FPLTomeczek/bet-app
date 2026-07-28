"use client";

import { useSyncExternalStore } from "react";
import { formatEventStart } from "./helpers";

const FALLBACK_TIME_ZONE = "Europe/Warsaw";

// The browser timezone is an external platform value, not React state. Reading it via
// useSyncExternalStore keeps SSR + the hydration render on the fallback (no mismatch),
// then swaps in the real zone — without an effect-driven setState and its cascading render.
// The three callbacks live at module scope on purpose: inline ones would make React
// re-subscribe on every render.
const subscribe = () => () => {};
const getTimeZone = () => Intl.DateTimeFormat().resolvedOptions().timeZone;
const getFallbackTimeZone = () => FALLBACK_TIME_ZONE;

export function EventTime({ startTime }: { startTime: string }) {
  const timeZone = useSyncExternalStore(
    subscribe,
    getTimeZone,
    getFallbackTimeZone
  );

  return <span>{formatEventStart(startTime, timeZone)}</span>;
}
