import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { OpportunityHierarchyDto } from '../types';

export function useOpportunityHierarchy(periodDays = 180) {
  return useQuery({
    queryKey: ['dashboard', 'opportunities', 'hierarchy', periodDays],
    queryFn: () =>
      api.get<OpportunityHierarchyDto>(
        `/dashboard/opportunities/hierarchy?periodDays=${periodDays}`,
      ),
  });
}
