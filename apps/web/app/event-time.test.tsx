import { render, screen } from "@testing-library/react";
import { renderToStaticMarkup } from "react-dom/server";
import { EventTime } from "./event-time";

function stubBrowserTimeZone(timeZone: string) {
  vi.spyOn(Intl.DateTimeFormat.prototype, "resolvedOptions").mockReturnValue({
    timeZone,
  } as Intl.ResolvedDateTimeFormatOptions);
}

describe("EventTime", () => {
  const instant = "2026-07-28T18:30:00Z"; // 20:30 in Warsaw (CEST), 14:30 in New York (EDT)

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("renders the instant in the browser timezone", () => {
    stubBrowserTimeZone("America/New_York");

    render(<EventTime startTime={instant} />);

    expect(screen.getByText(/14:30/)).toBeInTheDocument();
  });

  it("uses the Warsaw fallback when rendered on the server", () => {
    // getServerSnapshot must not touch the browser API — otherwise SSR output would
    // differ from the hydration render and React would throw a mismatch.
    stubBrowserTimeZone("America/New_York");

    const html = renderToStaticMarkup(<EventTime startTime={instant} />);

    expect(html).toContain("20:30");
    expect(html).not.toContain("14:30");
  });

  it("shifts the calendar day when the browser timezone crosses midnight", () => {
    stubBrowserTimeZone("Europe/Warsaw");

    render(<EventTime startTime="2026-01-15T23:30:00Z" />);

    expect(screen.getByText(/16 sty.*00:30/)).toBeInTheDocument();
  });
});
