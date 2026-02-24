import type { TimelineEventDto } from '@/types';
import { formatRelativeTime } from '@/lib/format';
import { getEntityPath } from '@/lib/navigation';

interface ActivityFeedItemProps {
  event: TimelineEventDto;
}

export function ActivityFeedItem({ event }: ActivityFeedItemProps) {
  const path =
    event.entityType && event.entityId
      ? getEntityPath(event.entityType, event.entityId)
      : null;

  return (
    <div className="gradient-accent-left border-b border-white/[0.04] py-3 pl-4 last:border-b-0">
      <p className="text-sm text-zinc-300">
        {event.eventDescription ?? event.eventType}
      </p>
      <div className="mt-1 flex items-center gap-2 text-xs text-zinc-500">
        {event.entityName && (
          <>
            {path ? (
              <a href={path} className="hover:text-nebula-violet">
                {event.entityName}
              </a>
            ) : (
              <span>{event.entityName}</span>
            )}
            <span>&middot;</span>
          </>
        )}
        {event.actorDisplayName && (
          <>
            <span>{event.actorDisplayName}</span>
            <span>&middot;</span>
          </>
        )}
        <span>{formatRelativeTime(event.occurredAt)}</span>
      </div>
    </div>
  );
}
