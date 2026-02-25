import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { useBrokerTimeline } from '@/hooks/useBrokerTimeline';
import { formatRelativeTime } from '@/lib/format';

interface BrokerTimelineTabProps {
  brokerId: string;
}

export function BrokerTimelineTab({ brokerId }: BrokerTimelineTabProps) {
  const { data: events, isLoading, isError, refetch } = useBrokerTimeline(brokerId);

  if (isLoading) {
    return (
      <div className="space-y-3">
        {Array.from({ length: 4 }).map((_, i) => (
          <Skeleton key={i} className="h-12 w-full" />
        ))}
      </div>
    );
  }

  if (isError) {
    return <ErrorFallback message="Unable to load timeline." onRetry={() => refetch()} />;
  }

  if (!events || events.length === 0) {
    return <p className="py-8 text-center text-sm text-zinc-500">No activity recorded.</p>;
  }

  return (
    <div className="space-y-3">
      {events.map((event) => (
        <div
          key={event.id}
          className="flex items-start gap-3 rounded-lg border border-zinc-800 p-3"
        >
          <div className="mt-1 h-2 w-2 flex-shrink-0 rounded-full bg-zinc-600" />
          <div className="min-w-0 flex-1">
            <p className="text-sm text-zinc-300">{event.eventDescription}</p>
            <div className="mt-1 flex gap-2 text-xs text-zinc-500">
              {event.actorDisplayName && <span>{event.actorDisplayName}</span>}
              <span>{formatRelativeTime(event.occurredAt)}</span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
