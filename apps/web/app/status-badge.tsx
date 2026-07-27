import { Badge } from "@/components/ui/badge";
import type { EventResponse } from "./lib/api";
import { STATUS_STYLES, STATUS_LABELS } from "./constants";

export function StatusBadge({ status }: { status: EventResponse["status"] }) {
  return (
    <Badge variant="secondary" className={STATUS_STYLES[status]}>
      {STATUS_LABELS[status]}
    </Badge>
  );
}
