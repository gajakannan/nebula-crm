import type { PipelineColorGroup, PipelineEntityType } from '@/types';
import { pipelineBg } from '@/lib/pipeline-colors';
import { cn } from '@/lib/utils';
import { Popover } from '@/components/ui/Popover';
import { PipelinePopoverContent } from './PipelinePopover';

interface PipelinePillProps {
  status: string;
  count: number;
  colorGroup: PipelineColorGroup;
  entityType: PipelineEntityType;
}

function formatStatus(status: string): string {
  return status.replace(/([A-Z])/g, ' $1').trim();
}

export function PipelinePill({ status, count, colorGroup, entityType }: PipelinePillProps) {
  const trigger = (
    <span
      className={cn(
        'inline-flex items-center gap-1.5 rounded-full px-3 py-1 text-xs font-medium text-white/90 transition-opacity hover:opacity-80',
        pipelineBg(colorGroup),
      )}
    >
      {formatStatus(status)}
      <span className="rounded-full bg-black/20 px-1.5 py-0.5 text-[10px] font-bold">
        {count}
      </span>
    </span>
  );

  return (
    <Popover trigger={trigger}>
      <PipelinePopoverContent entityType={entityType} status={status} />
    </Popover>
  );
}
