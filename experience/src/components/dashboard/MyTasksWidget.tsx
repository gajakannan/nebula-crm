import { useMyTasks } from '@/hooks/useMyTasks';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { TaskRow } from './TaskRow';

export function MyTasksWidget() {
  const { data, isLoading, isError, refetch } = useMyTasks();

  return (
    <Card>
      <CardHeader>
        <CardTitle>My Tasks</CardTitle>
      </CardHeader>

      {isLoading && (
        <div className="space-y-2">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      )}

      {isError && (
        <ErrorFallback
          message="Unable to load tasks"
          onRetry={() => refetch()}
        />
      )}

      {data && (
        <>
          {data.tasks.length === 0 ? (
            <p className="py-6 text-center text-sm text-zinc-500">
              No tasks assigned. You're all caught up.
            </p>
          ) : (
            <div className="space-y-1">
              {data.tasks.map((task) => (
                <TaskRow key={task.id} task={task} />
              ))}
            </div>
          )}
        </>
      )}
    </Card>
  );
}
