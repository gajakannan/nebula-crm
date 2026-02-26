import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { DashboardOpportunitiesDto } from '../types';

export function useDashboardOpportunities() {
  return useQuery({
    queryKey: ['dashboard', 'opportunities'],
    queryFn: () => api.get<DashboardOpportunitiesDto>('/dashboard/opportunities'),
  });
}
