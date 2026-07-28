export function formatEventStart(startTime: string, timeZone: string): string {
  return new Date(startTime).toLocaleString("pl-PL", {
    timeZone,
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
}
