export function formatEventStart(startTime: string): string {
  return new Date(startTime).toLocaleString("pl-PL", {
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
}
