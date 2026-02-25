import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { DashboardPipelineDto } from '@/types';

export function useDashboardPipeline() {
  return useQuery({
    queryKey: ['dashboard', 'pipeline'],
    queryFn: () => api.get<DashboardPipelineDto>('/dashboard/pipeline'),
  });
}
