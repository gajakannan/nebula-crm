import type { TaskSummaryDto } from '../types';
import { Badge } from '@/components/ui/Badge';
import { getEntityPath } from '@/lib/navigation';
import { cn } from '@/lib/utils';

interface TaskRowProps {
  task: TaskSummaryDto;
}

function formatDueDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
  });
}

export function TaskRow({ task }: TaskRowProps) {
  const linkedPath =
    task.linkedEntityType && task.linkedEntityId
      ? getEntityPath(task.linkedEntityType, task.linkedEntityId)
      : null;

  return (
    <div
      className={cn(
        'flex items-center justify-between rounded-lg px-3 py-2.5 transition-all',
        task.isOverdue
          ? 'bg-status-error/5 fx-shadow-task-overdue'
          : 'hover:bg-white/[0.03] fx-shadow-task-hover',
      )}
    >
      <div className="min-w-0 flex-1">
        <p
          className={cn(
            'truncate text-sm font-medium',
            task.isOverdue ? 'text-status-error' : 'text-text-primary',
          )}
        >
          {task.title}
        </p>
        {task.linkedEntityName && (
          <p className="mt-0.5 text-xs text-text-muted">
            {linkedPath ? (
              <a href={linkedPath} className="hover:text-nebula-violet">
                {task.linkedEntityName}
              </a>
            ) : (
              task.linkedEntityName
            )}
          </p>
        )}
      </div>

      <div className="ml-3 flex shrink-0 items-center gap-2">
        {task.dueDate && (
          <span
            className={cn(
              'text-xs',
              task.isOverdue ? 'font-medium text-status-error' : 'text-text-muted',
            )}
          >
            {formatDueDate(task.dueDate)}
          </span>
        )}
        <Badge variant={task.status === 'InProgress' ? 'info' : 'default'}>
          {task.status === 'InProgress' ? 'In Progress' : task.status}
        </Badge>
      </div>
    </div>
  );
}
