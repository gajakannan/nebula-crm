import type { NudgeCardDto } from '@/types';
import { Badge } from '@/components/ui/Badge';
import { canNavigateTo, getEntityPath } from '@/lib/navigation';
import { cn } from '@/lib/utils';

interface NudgeCardProps {
  nudge: NudgeCardDto;
  onDismiss: () => void;
}

const urgencyVariant: Record<string, 'error' | 'warning' | 'info'> = {
  OverdueTask: 'error',
  StaleSubmission: 'warning',
  UpcomingRenewal: 'info',
};

export function NudgeCard({ nudge, onDismiss }: NudgeCardProps) {
  const variant = urgencyVariant[nudge.nudgeType] ?? 'warning';
  const path = getEntityPath(nudge.linkedEntityType, nudge.linkedEntityId);
  const showCta = canNavigateTo(nudge.linkedEntityType);

  return (
    <div
      className={cn(
        'glass-card relative rounded-xl p-4',
        variant === 'error' && 'border-status-error/30 shadow-[0_0_20px_rgba(239,68,68,0.08)]',
        variant === 'warning' && 'border-status-warning/30 shadow-[0_0_20px_rgba(234,179,8,0.08)]',
        variant === 'info' && 'border-status-info/30 shadow-[0_0_20px_rgba(139,92,246,0.08)]',
      )}
    >
      <button
        onClick={onDismiss}
        className="absolute right-3 top-3 text-zinc-600 transition-colors hover:text-zinc-400"
        aria-label="Dismiss"
      >
        &times;
      </button>

      <div className="mb-2 flex items-center gap-2">
        <Badge variant={variant}>
          {nudge.nudgeType === 'OverdueTask'
            ? 'Overdue'
            : nudge.nudgeType === 'StaleSubmission'
              ? 'Stale'
              : 'Upcoming'}
        </Badge>
      </div>

      <h3 className="text-sm font-semibold text-zinc-200">{nudge.title}</h3>
      <p className="mt-1 text-xs text-zinc-400">{nudge.description}</p>

      {showCta && path && (
        <a
          href={path}
          className="mt-3 inline-block rounded-lg bg-gradient-to-r from-nebula-violet/20 to-nebula-fuchsia/20 px-3 py-1.5 text-xs font-medium text-nebula-violet transition-all hover:from-nebula-violet/30 hover:to-nebula-fuchsia/30 hover:shadow-[0_0_12px_rgba(139,92,246,0.2)]"
        >
          {nudge.ctaLabel}
        </a>
      )}
    </div>
  );
}
