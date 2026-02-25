import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { DashboardKpisDto } from '@/types';

export function useDashboardKpis() {
  return useQuery({
    queryKey: ['dashboard', 'kpis'],
    queryFn: () => api.get<DashboardKpisDto>('/dashboard/kpis'),
  });
}
