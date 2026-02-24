import type { PipelineMiniCardDto } from '@/types';
import { formatCurrency } from '@/lib/format';

interface PipelineMiniCardProps {
  item: PipelineMiniCardDto;
}

export function PipelineMiniCard({ item }: PipelineMiniCardProps) {
  return (
    <div className="flex items-center justify-between rounded-lg border border-white/[0.06] bg-white/[0.03] px-3 py-2 backdrop-blur-sm transition-colors hover:bg-white/[0.05]">
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-medium text-zinc-200">
          {item.entityName}
        </p>
        <div className="mt-0.5 flex items-center gap-2 text-xs text-zinc-500">
          {item.amount != null && <span>{formatCurrency(item.amount)}</span>}
          <span>{item.daysInStatus}d in status</span>
        </div>
      </div>
      {item.assignedUserInitials && (
        <span
          className="ml-2 flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-nebula-violet/10 text-[10px] font-medium text-nebula-violet/80"
          title={item.assignedUserDisplayName ?? undefined}
        >
          {item.assignedUserInitials}
        </span>
      )}
    </div>
  );
}
