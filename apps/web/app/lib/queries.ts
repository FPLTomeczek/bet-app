import { api, unwrap } from "./api";

// Named read surface for Server Components — one function per endpoint a screen actually
// uses. Also the place to attach React.cache, cache tags or param shaping when needed.
export function getEvents() {
  return unwrap(api.GET("/api/Events"));
}
