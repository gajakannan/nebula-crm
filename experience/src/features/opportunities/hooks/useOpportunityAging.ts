import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { OpportunityAgingDto, OpportunityEntityType } from '../types';

export function useOpportunityAging(
  entityType: OpportunityEntityType,
  periodDays = 180,
) {
  return useQuery({
    queryKey: ['dashboard', 'opportunities', entityType, 'aging', periodDays],
    queryFn: () =>
      api.get<OpportunityAgingDto>(
        `/dashboard/opportunities/aging?entityType=${entityType}&periodDays=${periodDays}`,
      ),
  });
}
