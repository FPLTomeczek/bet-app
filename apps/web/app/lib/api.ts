import createClient from "openapi-fetch";
import type { components, paths } from "./api-types";

// Generated from OpenAPI, never hand-written — regenerate with `npm run gen:api`.
export type EventResponse = components["schemas"]["EventResponse"];

// Server-only: reads run on the Node server, not the browser — no NEXT_PUBLIC_ prefix.
const API_BASE_URL = process.env.API_BASE_URL ?? "http://localhost:5075";

// Typed against the whole contract: URL, method, params and body are all checked, so a
// bad path or method is a compile error, not a runtime 404.
export const api = createClient<paths>({ baseUrl: API_BASE_URL });

// For reads only: openapi-fetch never throws and types `data` as optional; unwrap throws
// on a missing body so the caller gets T, not T | undefined (Next's error.tsx catches it).
// Don't use it for writes — a 400 there carries a typed error the form must read, not throw.
export async function unwrap<T>(
  call: Promise<{ data?: T; response: Response }>,
): Promise<T> {
  const { data, response } = await call;
  if (!data) throw new Error(`${response.status} ${response.statusText}`);
  return data;
}
