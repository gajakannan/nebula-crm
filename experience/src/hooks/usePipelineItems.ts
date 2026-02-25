import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { PipelineEntityType, PipelineItemsDto } from '@/types';

export function usePipelineItems(
  entityType: PipelineEntityType,
  status: string,
  enabled: boolean,
) {
  return useQuery({
    queryKey: ['dashboard', 'pipeline', entityType, status, 'items'],
    queryFn: () =>
      api.get<PipelineItemsDto>(
        `/dashboard/pipeline/${entityType}/${encodeURIComponent(status)}/items`,
      ),
    enabled,
  });
}
