import { formatEventStart } from "./helpers";

describe("formatEventStart", () => {
  const instant = "2026-07-28T18:30:00Z";

  it("renders the instant in the given timezone", () => {
    // Warsaw in July is CEST (UTC+2) → 20:30.
    const result = formatEventStart(instant, "Europe/Warsaw");

    expect(result).toContain("28");
    expect(result).toContain("lip");
    expect(result).toContain("20:30");
  });

  it("shows the same instant at different wall-clock times per timezone", () => {
    expect(formatEventStart(instant, "UTC")).toContain("18:30");
    expect(formatEventStart(instant, "America/New_York")).toContain("14:30"); // EDT, UTC-4
  });

  it("shifts the calendar day when the timezone crosses midnight", () => {
    // 23:30 UTC on Jan 15 → 00:30 on Jan 16 in Warsaw (CET, UTC+1).
    const result = formatEventStart("2026-01-15T23:30:00Z", "Europe/Warsaw");

    expect(result).toContain("16");
    expect(result).toContain("sty");
    expect(result).toContain("00:30");
  });
});
