import { useTimelineEvents } from '@/hooks/useTimelineEvents';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { ActivityFeedItem } from './ActivityFeedItem';

export function ActivityFeed() {
  const { data, isLoading, isError, refetch } = useTimelineEvents();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Activity</CardTitle>
      </CardHeader>

      {isLoading && (
        <div className="space-y-3">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-10 w-full" />
          ))}
        </div>
      )}

      {isError && (
        <ErrorFallback
          message="Unable to load activity feed"
          onRetry={() => refetch()}
        />
      )}

      {data && (
        <>
          {data.length === 0 ? (
            <p className="py-6 text-center text-sm text-zinc-500">
              No recent broker activity.
            </p>
          ) : (
            <div>
              {data.map((event) => (
                <ActivityFeedItem key={event.id} event={event} />
              ))}
            </div>
          )}
        </>
      )}
    </Card>
  );
}
