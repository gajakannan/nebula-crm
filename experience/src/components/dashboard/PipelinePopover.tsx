import type { PipelineEntityType } from '@/types';
import { usePipelineItems } from '@/hooks/usePipelineItems';
import { Skeleton } from '@/components/ui/Skeleton';
import { PipelineMiniCard } from './PipelineMiniCard';

interface PipelinePopoverProps {
  entityType: PipelineEntityType;
  status: string;
}

export function PipelinePopoverContent({ entityType, status }: PipelinePopoverProps) {
  const { data, isLoading, isError } = usePipelineItems(entityType, status, true);

  if (isLoading) {
    return (
      <div className="space-y-2">
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-12 w-full" />
      </div>
    );
  }

  if (isError || !data) {
    return <p className="text-xs text-zinc-500">Unable to load items</p>;
  }

  if (data.items.length === 0) {
    return <p className="text-xs text-zinc-500">No items</p>;
  }

  return (
    <div className="space-y-2">
      {data.items.map((item) => (
        <PipelineMiniCard key={item.entityId} item={item} />
      ))}
    </div>
  );
}
