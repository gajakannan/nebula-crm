import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { OpportunityOutcomesDto } from '../types';

export function useOpportunityOutcomes(periodDays = 180) {
  return useQuery({
    queryKey: ['dashboard', 'opportunities', 'outcomes', periodDays],
    queryFn: () =>
      api.get<OpportunityOutcomesDto>(
        `/dashboard/opportunities/outcomes?periodDays=${periodDays}`,
      ),
  });
}

