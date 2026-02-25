import type { PipelineEntityType, PipelineStatusCountDto } from '@/types';
import { PipelinePill } from './PipelinePill';

interface PipelineRowProps {
  label: string;
  entityType: PipelineEntityType;
  statuses: PipelineStatusCountDto[];
}

export function PipelineRow({ label, entityType, statuses }: PipelineRowProps) {
  return (
    <div>
      <h3 className="mb-2 text-xs font-medium text-zinc-400">{label}</h3>
      <div className="flex flex-wrap gap-2">
        {statuses.length === 0 ? (
          <p className="text-xs text-zinc-600">No items in pipeline</p>
        ) : (
          statuses.map((s) => (
            <PipelinePill
              key={s.status}
              status={s.status}
              count={s.count}
              colorGroup={s.colorGroup}
              entityType={entityType}
            />
          ))
        )}
      </div>
    </div>
  );
}
